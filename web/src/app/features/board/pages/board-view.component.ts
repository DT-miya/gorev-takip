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



// 🚀 GÖREV SÜRÜKLE - BIRAK MANTIĞI (Kolonlar Arası & Kolon İçi Aktarım)
drop(event: CdkDragDrop<TaskItem[]>, targetColumn: BoardColumn): void {
  // 1. Aynı kolonun aynı sırasına bırakıldıysa hiç işlem yapma
  if (
    event.previousContainer === event.container &&
    event.previousIndex === event.currentIndex
  ) {
    return;
  }

  // 2. Anlık UI Güncellemesi (Optimistic Update)
  if (event.previousContainer === event.container) {
    // Aynı kolon içinde sıra değiştirme
    moveItemInArray(
      event.container.data, 
      event.previousIndex, 
      event.currentIndex
    );
  } else {
    // Farklı kolona kart aktarma
    transferArrayItem(
      event.previousContainer.data,
      event.container.data,
      event.previousIndex,
      event.currentIndex
    );
  }

  // 3. Hedef konumdaki taşınan görevi tespit et
  const movedTask = event.container.data[event.currentIndex];
  
  // Backend sırası 1-based (1'den başlayan) olduğu için +1 ekliyoruz
  const newOrder = event.currentIndex + 1;

  // 4. API İsteği Gönder
  this.taskService.moveTask(movedTask.id, {
    targetColumnId: targetColumn.id,
    newOrder: newOrder
  }).subscribe({
    next: () => {
      // Başarılı olduğunda lokal task objesindeki columnId ve order'ı senkronize et
      movedTask.columnId = targetColumn.id;
      movedTask.order = newOrder;
    },
    error: (err: any) => {
      console.error('Görev taşıma/aktarma kaydedilemedi:', err);
      // Hata oluşursa arayüzü veritabanındaki gerçek durumuna geri döndür (Rollback)
      this.loadBoard();
    }
  });

  this.cdr.detectChanges();
}

// 🚀 KOLON SÜRÜKLE - BIRAK MANTIĞI
dropColumn(event: CdkDragDrop<BoardColumn[]>): void {
  // boardData veya columns yoksa ya da aynı sıraya bırakıldıysa işlem yapma
  if (!this.boardData?.columns || event.previousIndex === event.currentIndex) {
    return;
  }

  // 1. UI Güncellemesi (Optimistic Update)
  moveItemInArray(
    this.boardData.columns,
    event.previousIndex,
    event.currentIndex
  );

  // 2. Yeni kolon ID sıralaması
  const columnIds = this.boardData.columns.map(c => c.id);

  // 3. Proje ID'sini alıyoruz (boardData.projectId veya boardData.id)
  const currentProjectId = this.boardData.projectId || (this.boardData as any).id;

  // 4. Servisin beklediği payload objesini oluşturup gönderiyoruz
  this.columnService.reorderColumns({
    projectId: currentProjectId,
    orderedColumnIds: columnIds
  }).subscribe({
    next: () => {
      this.boardData?.columns.forEach((col, index) => col.order = index + 1);
    },
    error: (err: unknown) => {
      console.error('Kolon sırası kaydedilemedi:', err);
      this.loadBoard(); // Hata durumunda rollback
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