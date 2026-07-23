import { Component, signal, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ProjectService } from '@services/project.service';
import { Project } from '@models/project.model';

@Component({
  selector: 'app-project-edit-dialog',
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule

  ],
  templateUrl: './project-edit-dialog.html',
  styleUrl: './project-edit-dialog.css',
})
export class ProjectEditDialog {
  name = '';
  description = '';
  saving = signal(false);
  error = signal('');

  constructor(
    private projectService: ProjectService,
    private dialogRef: MatDialogRef<ProjectEditDialog>,
    @Inject(MAT_DIALOG_DATA) public data: { project: Project }    
  ) {
    this.name = data.project.name;
    this.description = data.project.description || '';
  }

  save(): void {
    if (!this.name.trim()) return;

    this.saving.set(true);
    this.projectService.updateProject(this.data.project.id, {
      name: this.name,
      description: this.description
    }).subscribe({
      next: (updated) => {
        this.dialogRef.close(updated);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Proje güncellenemedi');
        this.saving.set(false);
      }
    });
  }
  cancel(): void {
    this.dialogRef.close();
  }
}
