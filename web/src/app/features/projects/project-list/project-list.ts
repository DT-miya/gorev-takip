import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProjectService } from '../project.service';
import { Project } from '../../../core/models/project.model';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { ProjectCreateDialog } from '../project-create-dialog/project-create-dialog';

@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [CommonModule, MatButtonModule],
  templateUrl: './project-list.html',
  styleUrl: './project-list.css'
})
export class ProjectListComponent implements OnInit {
  projects = signal<Project[]>([]);
  loading = signal(true);

  constructor(private projectService: ProjectService, private dialog: MatDialog) {}

  ngOnInit(): void {
    console.log("ngOnit çalışıyor")
    this.projectService.getMyProjects().subscribe({
      next: (data) => {
        console.log("veri geldi", data)
        this.projects.set(data)
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Projeler yüklenemedi', err);
        this.loading.set(false);
      }
    });
  }

  openCreateDialog(): void {
  const dialogRef = this.dialog.open(ProjectCreateDialog, {
    width: '400px'
  });

  dialogRef.afterClosed().subscribe((created) => {
    if (created) {
      // yeni proje oluşturuldu, listeye ekle
      this.projects.set([...this.projects(), created]);
    }
  });
}
}