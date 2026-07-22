import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

// 🚀 CDK Importları
import { 
  DragDropModule, 
  CdkDragDrop, 
  moveItemInArray, 
  transferArrayItem 
} from '@angular/cdk/drag-drop';

import { 
  TaskService, 
  BoardFullResponse, 
  BoardColumn, 
  TaskItem 
} from '@core/services/task.service';
import { ColumnService } from '@core/services/column.service'; // 👈 Import edildiğinden emin ol

@Component({
  selector: 'app-board-view',
  standalone: true,
  imports: [CommonModule, FormsModule, DragDropModule],
  templateUrl: './board-view.component.html',
  styleUrls: ['./board-view.component.css']
})
export class BoardViewComponent implements OnInit {
  projectId!: number;
  boardData?: BoardFullResponse;
  isLoading = true;

  activeColumnIdForNewTask: number | null = null;
  newTaskTitle: string = '';

  constructor(
    private route: ActivatedRoute,
    private taskService: TaskService,
    private columnService: ColumnService, // 📍 DÜZELTME 1: Constructor'a enjekte edildi
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
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Pano yüklenirken hata oluştu:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
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
      orderedColumnIds: this.boardData.columns.map(col => col.id)
    };

    this.columnService.reorderColumns(reorderPayload).subscribe({
      // 📍 DÜZELTME 2: 'err: any' şeklinde tipi belirtildi
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
    }
    this.cdr.detectChanges();
  }

  addTask(column: BoardColumn): void {
    if (!this.newTaskTitle.trim()) return;

    const request = {
      columnId: column.id,
      title: this.newTaskTitle.trim(),
      priority: 'Medium'
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