import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../../core/services/admin.service';
import { AdminStats } from '../../../core/models/admin.model';

@Component({
  selector: 'app-admin-stats',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-stats.html',
  styleUrl: './admin-stats.css'
})
export class AdminStatsComponent implements OnInit {
  stats = signal<AdminStats | null>(null);
  loading = signal(true);

  message = signal<string | null>(null);
  selectedTab = signal<'tasks' | 'members' | null>(null);
  taskData = signal<any[]>([]);
  memberData = signal<any[]>([]);

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.adminService.getStats().subscribe({
      next: (data) => {
        this.stats.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('İstatistikler yüklenemedi', err);
        this.loading.set(false);
      }
    });
  }
}