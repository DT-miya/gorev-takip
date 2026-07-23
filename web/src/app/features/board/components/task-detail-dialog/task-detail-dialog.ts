import { Component, EventEmitter, Input, Output, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import type { TaskItem, UpdateTaskRequest } from '@services/task.service';
import type { Member } from '@services/project.service';

@Component({
  selector: 'app-task-detail-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-detail-dialog.html',
  styleUrls: ['./task-detail-dialog.css']
})
export class TaskDetailDialogComponent implements OnInit, OnChanges {
  @Input() task!: TaskItem;
  @Input() members: Member[] = [];
  @Input() isOpen = false;

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<UpdateTaskRequest>();
  @Output() delete = new EventEmitter<number>();

  formData: UpdateTaskRequest = {
    title: '',
    description: '',
    priority: 'Medium',
    assigneeId: null,
    dueDate: null
  };

  ngOnInit(): void {
    this.initForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['task'] && this.task) {
      this.initForm();
    }
  }

  private initForm(): void {
    if (!this.task) return;
    this.formData = {
      title: this.task.title || '',
      description: this.task.description || '',
      priority: this.task.priority || 'Medium',
      assigneeId: this.task.assigneeId ?? null,
      dueDate: this.task.dueDate ? this.task.dueDate.split('T')[0] : null
    };
  }

  onSave(): void {
    if (!this.formData.title.trim()) return;
    this.save.emit(this.formData);
  }

  onDelete(): void {
    if (confirm('Bu görevi silmek istediğinize emin misiniz?')) {
      this.delete.emit(this.task.id);
    }
  }

  onClose(): void {
    this.close.emit();
  }
}