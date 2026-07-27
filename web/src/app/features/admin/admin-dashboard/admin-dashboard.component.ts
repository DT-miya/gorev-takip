import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '@services/admin.service';
import type { AdminUser, AdminProject } from '@services/admin.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  users: AdminUser[] = [];
  projects: AdminProject[] = [];
  isLoading = true;

  activeTab: 'projects' | 'users' = 'projects';

  constructor(
    private adminService: AdminService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;

    // Kullanıcıları ve projeleri çekiyoruz
    this.adminService.getUsers().subscribe({
      next: (users) => {
        this.users = users;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Kullanıcılar yüklenemedi:', err)
    });

    this.adminService.getProjects().subscribe({
      next: (projects) => {
        this.projects = projects;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Projeler yüklenemedi:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  deleteProject(projectId: number): void {
    if (confirm('Bu projeyi ve projeye ait tüm verileri silmek istediğinize emin misiniz?')) {
      this.adminService.deleteProject(projectId).subscribe({
        next: () => this.loadData(),
        error: (err) => console.error('Proje silinemedi:', err)
      });
    }
  }

  get adminCount(): number {
    return this.users.filter(u => u.role === 'Admin').length;
  }
}