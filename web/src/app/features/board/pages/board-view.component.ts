import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

// 🚀 Servis & Model Importları
import { TaskService } from '@services/task.service';
import type { BoardFullResponse, BoardColumn, TaskItem, UpdateTaskRequest } from '@services/task.service';
import { ColumnService } from '@services/column.service';
import { ProjectService } from '@services/project.service';
import type { Member } from '@services/project.service';

// 🚀 Component Import
import { TaskDetailDialogComponent } from '../components/task-detail-dialog/task-detail-dialog';

// 🚀 CDK Drag & Drop Importları
import { 
  DragDropModule, 
  CdkDragDrop, 
  moveItemInArray, 
  transferArrayItem 
} from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-board-view',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    DragDropModule, 
    TaskDetailDialogComponent
  ],
  templateUrl: './board-view.component.html',
  styleUrls: ['./board-view.component.css']
})
export class BoardViewComponent implements OnInit {
  projectId!: number;
  boardData?: BoardFullResponse;
  isLoading = true;

  activeColumnIdForNewTask: number | null = null;
  newTaskTitle: string = '';
  newTaskPriority: string = 'Medium';
  priorityOptions = ['Low', 'Medium', 'High'];

  // 🚀 Task Detail Dialog State & Data
  isDetailModalOpen = false;
  selectedTask: TaskItem | null = null;
  projectMembers: Member[] = [];

  constructor(
    private route: ActivatedRoute,
    private taskService: TaskService,
    private columnService: ColumnService,
    private projectService: ProjectService, // 📍 ProjectService eklendi
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.projectId = +params['id'];
      if (this.projectId) {
        this.loadBoard();
      }
    });
  }

  loadBoard(): void {
    this.isLoading = true;
    this.taskService.getFullBoard(this.projectId).subscribe({
      next: (data: BoardFullResponse) => {
        this.boardData = data;
        this.isLoading = false;
        this.loadMembers(); // 📍 Pano yuklenince proje üyelerini de cekiyoruz
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Pano yüklenirken hata oluştu:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // 🚀 PROJE ÜYELERİNİ ÇEKME (Dropdown için)
 loadMembers(): void {
    this.projectService.getMembers(this.projectId).subscribe({
      next: (members: Member[]) => {
        this.projectMembers = members;
        this.cdr.detectChanges();
      },
      error: (err: any) => console.error('Üyeler yüklenemedi:', err)
    });
  }

  // 🚀 TASK DETAIL DIALOG İŞLEMLERİ
  openTaskDetail(task: TaskItem): void {
    this.selectedTask = task;
    this.isDetailModalOpen = true;
    this.cdr.detectChanges();
  }

  closeTaskDetail(): void {
    this.isDetailModalOpen = false;
    this.selectedTask = null;
    this.cdr.detectChanges();
  }

  saveTaskDetail(request: UpdateTaskRequest): void {
    if (!this.selectedTask) return;

    this.taskService.updateTask(this.selectedTask.id, request).subscribe({
      next: () => {
        this.loadBoard(); // Ekrandaki verileri güncelle
        this.closeTaskDetail();
      },
      error: (err: any) => console.error('Görev güncellenemedi:', err)
    });
  }

  deleteTaskDetail(taskId: number): void {
    this.taskService.deleteTask(taskId).subscribe({
      next: () => {
        this.loadBoard(); // Ekrandaki verileri güncelle
        this.closeTaskDetail();
      },
      error: (err: any) => console.error('Görev silinemedi:', err)
    });
  }

  // 🚀 KOLON SIRALAMA MANTIĞI
  dropColumn(event: CdkDragDrop<BoardColumn[]>): void {
    if (!this.boardData || !this.boardData.columns) return;

    moveItemInArray(
      this.boardData.columns, 
      event.previousIndex, 
      event.currentIndex
    );

    const reorderPayload = {
      projectId: this.projectId,
      orderedColumnIds: this.boardData.columns.map((col: any) => col.id)
    };

    this.columnService.reorderColumns(reorderPayload).subscribe({
      error: (err: any) => {
        console.error('Kolon sırası kaydedilemedi:', err);
        this.loadBoard();
      }
    });

    this.cdr.detectChanges();
  }

  // 🚀 GÖREV SÜRÜKLE - BIRAK MANTIĞI
  drop(event: CdkDragDrop<TaskItem[]>, targetColumn: BoardColumn): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(
        event.container.data, 
        event.previousIndex, 
        event.currentIndex
      );
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );
    }

    const movedTask = event.container.data[event.currentIndex];
    
    this.taskService.moveTask(movedTask.id, {
      targetColumnId: targetColumn.id,
      newOrder: event.currentIndex + 1
    }).subscribe({
      error: (err: any) => {
        console.error('Görev taşıma kaydedilemedi:', err);
        this.loadBoard();
      }
    });

    this.cdr.detectChanges();
  }

  toggleNewTaskForm(columnId: number): void {
    if (this.activeColumnIdForNewTask === columnId) {
      this.activeColumnIdForNewTask = null;
    } else {
      this.activeColumnIdForNewTask = columnId;
      this.newTaskTitle = '';
      this.newTaskPriority = 'Medium'; // Varsayılan öncelik 'Medium'
    }
    this.cdr.detectChanges();
  }

  addTask(column: BoardColumn): void {
    if (!this.newTaskTitle.trim()) return;

    const request = {
      columnId: column.id,
      title: this.newTaskTitle.trim(),
      priority: this.newTaskPriority || 'Medium', // Varsayılan öncelik 'Medium'
    };

    this.taskService.createTask(request).subscribe({
      next: (newTask: TaskItem) => {
        column.tasks.push(newTask);
        this.newTaskTitle = '';
        this.activeColumnIdForNewTask = null;
        this.cdr.detectChanges();
      },
      error: (err: any) => console.error('Görev eklenemedi:', err)
    });
  }
}