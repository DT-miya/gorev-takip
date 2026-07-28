import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, signal } from '@angular/core';
import { AdminProject } from '@models/admin.model';
import { AdminService } from '@services/admin.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-projects',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-projects.html',
  styleUrl: './admin-projects.css',
})
export class AdminProjects implements OnInit {
  projects = signal<AdminProject[]>([]);
  searchTerm = signal('');
  minColumns = signal<number | null>(null);
  minTasks = signal<number | null>(null);
  minMembers = signal<number | null>(null);
  currentPage = signal(1);
  totalCount = signal(0);
  loading = signal(true);
  error = signal<string | null>(null);
  pageSize = 20;
  selectedTab = signal<'tasks' | 'members' | null>(null);
  selectedProjectId = signal<number | null>(null);
  taskData = signal<any[]>([]);
  memberData = signal<any[]>([]);
  message = signal<string | null>(null);

  totalPages = computed(() => {
    return Math.max(1, Math.ceil(this.totalCount() / this.pageSize));
  });

  constructor(private adminService: AdminService, private router: Router) {}

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.loading.set(true);
    this.error.set(null);

    this.adminService.getProjectsPage({
      page: this.currentPage(),
      pageSize: this.pageSize,
      search: this.searchTerm().trim(),
      minColumns: this.minColumns(),
      minTasks: this.minTasks(),
      minMembers: this.minMembers(),
      showArchived: this.showArchived()
    }).subscribe({
      next: (response) => {
        this.projects.set(response.items);
        this.totalCount.set(response.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Projeler yüklenemedi', err);
        this.error.set('Projeler yüklenemedi.');
        this.loading.set(false);
      }
    });
  }

  setSearchTerm(value: string): void {
    this.searchTerm.set(value);
    this.currentPage.set(1);
    this.loadProjects();
  }

  setMinColumns(value: string): void {
    this.minColumns.set(this.toOptionalNumber(value));
    this.currentPage.set(1);
    this.loadProjects();
  }

  setMinTasks(value: string): void {
    this.minTasks.set(this.toOptionalNumber(value));
    this.currentPage.set(1);
    this.loadProjects();
  }

  setMinMembers(value: string): void {
    this.minMembers.set(this.toOptionalNumber(value));
    this.currentPage.set(1);
    this.loadProjects();
  }

  previousPage(): void {
    if (this.currentPage() > 1) {
      this.currentPage.set(this.currentPage() - 1);
      this.loadProjects();
    }
  }

  nextPage(): void {
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.set(this.currentPage() + 1);
      this.loadProjects();
    }
  }

  private toOptionalNumber(value: string): number | null {
    if (value === '') {
      return null;
    }

    const parsed = Number(value);
    return Number.isNaN(parsed) ? null : parsed;
  }

 openProjectTasks(projectId: number): void {
  this.selectedProjectId.set(projectId);
  this.selectedTab.set('tasks');
  this.message.set(null);

  this.adminService.getProjectTasks(projectId).subscribe({
    next: (res) => {
      this.taskData.set(res.columns ?? []);
    },
    error: (err) => {
      console.error('Görevler yüklenemedi', err);
      this.message.set('Görevler yüklenemedi.');
    }
  });
 }

openProjectMembers(projectId: number): void {
  this.selectedProjectId.set(projectId);
  this.selectedTab.set('members');
  this.message.set(null);

  this.adminService.getProjectMembers(projectId).subscribe({
    next: (res) => {
      this.memberData.set(res);
    },
    error: (err) => {
      console.error('Üyeler yüklenemedi', err);
      this.message.set('Üyeler yüklenemedi.');
    }
  });
}

archiveProject(projectId: number): void {
  this.selectedProjectId.set(projectId);
  this.selectedTab.set(null);
  

  this.adminService.archiveProject(projectId).subscribe({
    next: () => {
      this.message.set("Proje başarıyla arşivlendi.");
      this.loadProjects();
    },
    error: (err) => {
      console.error('Proje arşivlenemedi', err);
      this.message.set('Proje arşivlenemedi.');
    }
  });

}

showArchived = signal(false);

toggleArchivedView(): void {
  this.showArchived.set(!this.showArchived());
  this.currentPage.set(1);
  this.loadProjects();
}

unarchiveProject(projectId: number): void {
  this.adminService.unarchiveProject(projectId).subscribe({
    next: () => {
      this.message.set("Proje geri getirildi.");
      this.loadProjects();
    },
    error: (err) => {
      console.error('Proje geri getirilemedi', err);
      this.message.set('Proje geri getirilemedi.');
    }
  });
}

  closeModal(): void {
      this.selectedTab.set(null);
  }
  
}
