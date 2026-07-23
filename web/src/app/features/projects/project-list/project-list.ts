import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProjectService } from '@services/project.service';
import { Project } from '@models/project.model';
import { MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { ProjectCreateDialog } from '../project-create-dialog/project-create-dialog';
import { ProjectMembersDialog } from '../project-members-dialog/project-members-dialog';
import { Router } from '@angular/router';

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

  constructor(private projectService: ProjectService, private dialog: MatDialog, private router: Router) {}

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

deleteProject(id: number, event: Event): void {
  event.stopPropagation();

  if (!confirm("Bu projeyi silmek istediğine emin misin ?")) {
    return;
  }

  this.projectService.deleteProject(id).subscribe({
    next: () => {
      this.projects.set(this.projects().filter(p => p.id !== id));
    },
    error: (err) => {
      console.error("Proje silinemedi", err)
    }
  });
}

openMembersDialog(projectId: number, event: Event): void {
  event.stopPropagation();
  this.dialog.open(ProjectMembersDialog, {
    width: '500px',
    data: { projectId }
  });
}

openBoard(projectId: number): void {
  this.router.navigate(['/projects', projectId, 'board']);
}
}