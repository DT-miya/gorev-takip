import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ProjectService } from '../project.service';

@Component({
  selector: 'app-project-create-dialog',
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './project-create-dialog.html',
  styleUrl: './project-create-dialog.css',
})
export class ProjectCreateDialog {
projectName = '';
description = '';
saving =signal(false);

constructor(
  private projectService: ProjectService,
  private dialogRef: MatDialogRef<ProjectCreateDialog>
) {}

save(): void {
  if (!this.projectName.trim()) return;

  this.saving.set(true);
  this.projectService.createProject({
    name: this.projectName,
    description: this.description
  }).subscribe({
    next: (created) => {
      this.dialogRef.close(created);
    },
    error: (err) => {
      console.error("Proje oluşturulamadı.", err);
      this.saving.set(false);
    }
  });
}

cancel(): void {
  this.dialogRef.close();
}
}