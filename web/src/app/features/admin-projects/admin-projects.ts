import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, signal, ViewChild, ElementRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminProject } from '@models/admin.model';
import { AdminService } from '@services/admin.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-projects',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-projects.html',
  styleUrl: './admin-projects.css'
})
export class AdminProjects implements OnInit {
  // --- SIGNAL DEFINITIONS ---
  projects = signal<AdminProject[]>([]);
  
  projectNameSearch = signal('');
  ownerSearch = signal('');

  minColumns = signal<number | null>(null);
  minTasks = signal<number | null>(null);
  minMembers = signal<number | null>(null);
  currentPage = signal(1);
  totalCount = signal(0);
  loading = signal(true);
  error = signal<string | null>(null);
  pageSize = 20;

  showArchived = signal(false);

  selectedTab = signal<'tasks' | 'members' | null>(null);
  selectedProjectId = signal<number | null>(null);
  taskData = signal<any[]>([]);
  memberData = signal<any[]>([]);
  message = signal<string | null>(null);

  private searchTimer: any;

  // Pop-up Filtre Görünürlükleri
  showNameFilter = signal(false);
  showOwnerFilter = signal(false);

  @ViewChild('searchInput') searchInput?: ElementRef<HTMLInputElement>;

  totalPages = computed(() => {
    return Math.max(1, Math.ceil(this.totalCount() / this.pageSize));
  });

  constructor(private adminService: AdminService, private router: Router) {}

  ngOnInit(): void {
    this.loadProjects();
  }

  toggleNameFilter(event: Event): void {
    event.stopPropagation();
    this.showOwnerFilter.set(false);
    this.showNameFilter.set(!this.showNameFilter());
  }

  toggleOwnerFilter(event: Event): void {
    event.stopPropagation();
    this.showNameFilter.set(false);
    this.showOwnerFilter.set(!this.showOwnerFilter());
  }

  loadProjects(): void {
    this.loading.set(true);
    this.error.set(null);

    this.adminService
      .getProjectsPage({
        page: this.currentPage(),
        pageSize: this.pageSize,
        projectName: this.projectNameSearch().trim(),
        ownerSearch: this.ownerSearch().trim(),
        minColumns: this.minColumns(),
        minTasks: this.minTasks(),
        minMembers: this.minMembers(),
        showArchived: this.showArchived(),
      })
      .subscribe({
        next: (response: any) => {
          let list: AdminProject[] = [];
          let total = 0;

          if (Array.isArray(response)) {
            list = response;
            total = response.length;
          } else if (response && typeof response === 'object') {
            list = response.items ?? response.Items ?? response.data ?? response.Data ?? [];
            total = response.totalCount ?? response.TotalCount ?? response.total ?? response.Total ?? list.length;
          }

          this.projects.set(list);
          this.totalCount.set(total);
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Projeler yüklenemedi', err);
          this.error.set('Projeler yüklenemedi.');
          this.loading.set(false);
        },
      });
  }

  onProjectNameChange(value: string): void {
    this.projectNameSearch.set(value);
    this.triggerDebouncedSearch();
  }

  onOwnerSearchChange(value: string): void {
    this.ownerSearch.set(value);
    this.triggerDebouncedSearch();
  }

  private triggerDebouncedSearch(): void {
    if (this.searchTimer) {
      clearTimeout(this.searchTimer);
    }
    this.searchTimer = setTimeout(() => {
      this.currentPage.set(1);
      this.loadProjects();
    }, 400);
  }

  onFilterChange(): void {
    this.triggerDebouncedSearch();
  }

  setMinColumns(value: string | number | Event): void {
    this.minColumns.set(this.toOptionalNumber(value));
    this.currentPage.set(1);
    this.loadProjects();
  }

  setMinTasks(value: string | number | Event): void {
    this.minTasks.set(this.toOptionalNumber(value));
    this.currentPage.set(1);
    this.loadProjects();
  }

  setMinMembers(value: string | number | Event): void {
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

  private toOptionalNumber(value: any): number | null {
    if (value instanceof Event) {
      value = (value.target as HTMLInputElement).value;
    }
    if (value === '' || value === null || value === undefined) {
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
      next: (res: any) => {
        // 1. Gelen yanıtın ham veri listesini yakala
        const rawTasks: any[] = Array.isArray(res)
          ? res
          : (res?.columns ?? res?.tasks ?? res?.data ?? []);

        // 2. Eğer backend zaten gruplanmış/kolonlu yapı gönderiyorsa direkt ata
        if (rawTasks.length > 0 && (rawTasks[0]?.tasks || rawTasks[0]?.columns)) {
          this.taskData.set(rawTasks);
          return;
        }

        // 3. Düz gelen diziyi columnName'e göre Map yapısı ile grupla
        const groupedMap = new Map<string, any[]>();

        for (const task of rawTasks) {
          const colName = task.columnName || task.columnTitle || task.status || 'Genel Görevler';
          if (!groupedMap.has(colName)) {
            groupedMap.set(colName, []);
          }
          groupedMap.get(colName)!.push(task);
        }

        // 4. HTML şablonunun (column.title & column.tasks) formatına dönüştür
        const formattedColumns = Array.from(groupedMap.entries()).map(([columnName, tasks], index) => ({
          id: index + 1,
          title: columnName,
          tasks: tasks
        }));

        this.taskData.set(formattedColumns);
      },
      error: (err) => {
        console.error('Görevler yüklenemedi', err);
        this.message.set('Görevler yüklenemedi.');
      },
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
      },
    });
  }

  archiveProject(projectId: number): void {
    this.selectedProjectId.set(projectId);
    this.selectedTab.set(null);

    this.adminService.archiveProject(projectId).subscribe({
      next: () => {
        this.message.set('Proje başarıyla arşivlendi.');
        this.loadProjects();
      },
      error: (err) => {
        console.error('Proje arşivlenemedi', err);
        this.message.set('Proje arşivlenemedi.');
      },
    });
  }

  toggleArchivedView(): void {
    this.showArchived.set(!this.showArchived());
    this.currentPage.set(1);
    this.loadProjects();
  }

  unarchiveProject(projectId: number): void {
    this.adminService.unarchiveProject(projectId).subscribe({
      next: () => {
        this.message.set('Proje geri getirildi.');
        this.loadProjects();
      },
      error: (err) => {
        console.error('Proje geri getirilemedi', err);
        this.message.set('Proje geri getirilemedi.');
      },
    });
  }

  closeModal(): void {
    this.selectedTab.set(null);
  }
}