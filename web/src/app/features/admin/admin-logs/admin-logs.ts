import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { AdminService } from '@services/admin.service';
import { ActivityLog } from '@models/admin.model';

@Component({
  selector: 'app-admin-logs',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-logs.component.html',
  styleUrls: ['./admin-logs.css']
})
export class AdminLogsComponent implements OnInit {
  logs: ActivityLog[] = [];

  // Sayfalandırma Değişkenleri
  currentPage = 1;
  pageSize = 20;
  totalCount = 0;
  totalPages = 1;
  isLoading = false;

  searchEmail = '';
  searchAction = '';
  searchDescription = '';


  private searchSubject = new Subject<string>();

  private adminService = inject(AdminService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.loadLogs();

    // Debounce mekanizması: Yazma bittikten 300ms sonra istek atar
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(search => {
      this.searchEmail = this.searchEmail?.trim();
      this.searchAction = this.searchAction?.trim();
      this.searchDescription = this.searchDescription?.trim();
      this.currentPage = 1; // Yeni aramada 1. sayfaya sıfırla
      this.loadLogs();
    });
  }

  onSearchChange(searchValue: string): void {
    this.searchSubject.next(searchValue);
  }


  onEmailChange(value: string): void {
  this.searchEmail = value;
  this.onSearchChange(this.searchEmail);
}

onActionChange(value: string): void {
  this.searchAction = value;
  this.onSearchChange(this.searchAction);
}

onDescriptionChange(value: string): void {
  this.searchDescription = value;
  this.onSearchChange(this.searchDescription);
}




  loadLogs(): void {
    this.isLoading = true;
    this.adminService.getLogs(this.searchEmail, this.searchAction, this.searchDescription, this.currentPage, this.pageSize).subscribe({
      next: (result) => {
        this.logs = result.items || [];
        this.totalCount = result.totalCount || 0;
        this.totalPages = result.totalPages || 1;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Loglar yüklenemedi:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadLogs();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadLogs();
    }
  }

actionTypes: string[] = [
    'ROLE_CHANGE',
    'PROJECT_CREATED',
    'PROJECT_DELETED',
    'TASK_MOVED',
    'TASK_CREATED',
    'USER_LOGIN',
    'USER_REGISTER',
    'USER_LOGOUT',
    'USER_CREATE_PROJECT',
    'USER_DELETE_PROJECT',
    'USER_UPDATE_PROJECT',
    'USER_ADD_PROJECT_MEMBER',
    'USER_REMOVE_PROJECT_MEMBER'
  ];


  // Action türüne göre rozet renk sınıfı belirler
  getActionClass(action: string): string {
    switch (action?.toUpperCase()) {
      case 'ROLE_CHANGE':
        return 'action-role';
      case 'PROJECT_CREATED':
      case 'PROJECT_DELETED':
        return 'action-project';
      case 'TASK_MOVED':
      case 'TASK_CREATED':
        return 'action-task';
      case 'USER_LOGIN':
      case 'USER_REGISTER':
      case 'USER_LOGOUT':
        return 'action-user';
      case 'USER_CREATE_PROJECT':
      case 'USER_DELETE_PROJECT':
      case 'USER_UPDATE_PROJECT':
        return 'action-user-project';
      case 'USER_ADD_PROJECT_MEMBER':
      case 'USER_REMOVE_PROJECT_MEMBER':
        return 'action-user-project-member';
      default:
        return 'action-default';
    }
  }
}