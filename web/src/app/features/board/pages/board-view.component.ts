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

// 🚀 Dialog Component Import
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

  // 🚀 Kolon Yönetimi State
  isAddingColumn = false;
  newColumnTitle = '';
  editingColumnId: number | null = null;
  editingColumnTitle = '';

  // 🚀 Task Detail Dialog State & Data
  isDetailModalOpen = false;
  selectedTask: TaskItem | null = null;
  projectMembers: Member[] = [];

  constructor(
    private route: ActivatedRoute,
    private taskService: TaskService,
    private columnService: ColumnService,
    private projectService: ProjectService,
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
        this.loadMembers();
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Pano yüklenirken hata oluştu:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // 🚀 PROJE ÜYELERİNİ ÇEKME
  loadMembers(): void {
    this.projectService.getMembers(this.projectId).subscribe({
      next: (members: Member[]) => {
        this.projectMembers = members;
        this.cdr.detectChanges();
      },
      error: (err: any) => console.error('Üyeler yüklenemedi:', err)
    });
  }

  // 🚀 KOLON SIRALAMA MANTIĞI (Tekil Metot)
  dropColumn(event: CdkDragDrop<BoardColumn[]>): void {
    if (!this.boardData || !this.boardData.columns) return;
    if (event.previousIndex === event.currentIndex) return;

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

  // 🚀 GÖREV SÜRÜKLE - BIRAK MANTIĞI (Sıralama Kaybını Önleyen Versiyon)
drop(event: CdkDragDrop<TaskItem[]>, targetColumn: BoardColumn): void {
  // 1. Aynı yere bırakıldıysa işlem yapma
  if (
    event.previousContainer === event.container &&
    event.previousIndex === event.currentIndex
  ) {
    return;
  }

  // 2. Frontend dizisini anlık olarak güncelle
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

  // 3. Taşınan görevi ve yeni sırasını tespit et
  const movedTask = event.container.data[event.currentIndex];
  
  // Backend sırası 1-based (1'den başlayan) kabul ediliyorsa + 1 ekliyoruz
  const calculatedNewOrder = event.currentIndex + 1;

  // 4. API İsteği
  this.taskService.moveTask(movedTask.id, {
    targetColumnId: targetColumn.id,
    newOrder: calculatedNewOrder
  }).subscribe({
    next: () => {
      // Başarılı olduğunda lokal objede de columnId ve order'ı güncelle
      movedTask.columnId = targetColumn.id;
      movedTask.order = calculatedNewOrder;
    },
    error: (err: any) => {
      console.error('Görev taşıma kaydedilemedi:', err);
      // Hata durumunda eski sırayı korumak için panoyu yeniden yükle
      this.loadBoard();
    }
  });

  this.cdr.detectChanges();
}

  // 🚀 KOLON EKLEME / DÜZENLEME / SİLME İŞLEMLERİ
  toggleAddColumnForm(): void {
    this.isAddingColumn = !this.isAddingColumn;
    this.newColumnTitle = '';
    this.cdr.detectChanges();
  }

  addColumn(): void {
    if (!this.newColumnTitle.trim()) return;

    this.columnService.createColumn({
      projectId: this.projectId,
      title: this.newColumnTitle.trim()
    }).subscribe({
      next: () => {
        this.isAddingColumn = false;
        this.newColumnTitle = '';
        this.loadBoard();
      },
      error: (err: any) => console.error('Kolon eklenemedi:', err)
    });
  }

  startRenameColumn(column: BoardColumn, event: Event): void {
    event.stopPropagation();
    this.editingColumnId = column.id;
    this.editingColumnTitle = column.title;
    this.cdr.detectChanges();
  }

  cancelRenameColumn(): void {
    this.editingColumnId = null;
    this.editingColumnTitle = '';
    this.cdr.detectChanges();
  }

  saveRenameColumn(columnId: number): void {
    if (!this.editingColumnTitle.trim()) return;

    this.columnService.updateColumn(columnId, { title: this.editingColumnTitle.trim() }).subscribe({
      next: () => {
        this.editingColumnId = null;
        this.loadBoard();
      },
      error: (err: any) => console.error('Kolon adı güncellenemedi:', err)
    });
  }

  deleteColumn(columnId: number, event: Event): void {
    event.stopPropagation();
    if (confirm('Bu kolonu ve içindeki tüm görevleri silmek istediğinize emin misiniz?')) {
      this.columnService.deleteColumn(columnId).subscribe({
        next: () => this.loadBoard(),
        error: (err: any) => console.error('Kolon silinemedi:', err)
      });
    }
  }

  // 🚀 TASK DETAIL DIALOG İŞLEMLERİ
  openTaskDetail(task: TaskItem): void {
    this.selectedTask = task;
    this.isDetailModalOpen = true;
    this.cdr.detectChanges();
  }

  openTaskDetailDialog(task: TaskItem): void {
    this.openTaskDetail(task);
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
        this.loadBoard();
        this.closeTaskDetail();
      },
      error: (err: any) => console.error('Görev güncellenemedi:', err)
    });
  }

  deleteTaskDetail(taskId: number): void {
    this.taskService.deleteTask(taskId).subscribe({
      next: () => {
        this.loadBoard();
        this.closeTaskDetail();
      },
      error: (err: any) => console.error('Görev silinemedi:', err)
    });
  }

  // 🚀 HIZLI GÖREV EKLEME
  toggleNewTaskForm(columnId: number): void {
    if (this.activeColumnIdForNewTask === columnId) {
      this.activeColumnIdForNewTask = null;
    } else {
      this.activeColumnIdForNewTask = columnId;
      this.newTaskTitle = '';
      this.newTaskPriority = 'Medium';
    }
    this.cdr.detectChanges();
  }

  addTask(column: BoardColumn): void {
    if (!this.newTaskTitle.trim()) return;

    const request = {
      columnId: column.id,
      title: this.newTaskTitle.trim(),
      priority: this.newTaskPriority || 'Medium'
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

  // 🚀 YARDIMCI METOTLAR
  isOverdue(dueDateStr?: string | null): boolean {
    if (!dueDateStr) return false;
    const dueDate = new Date(dueDateStr);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return dueDate < today;
  }

  getInitials(name?: string | null): string {
    if (!name) return '?';
    const parts = name.trim().split(' ');
    if (parts.length >= 2) {
      return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
    }
    return parts[0][0].toUpperCase();
  }
}