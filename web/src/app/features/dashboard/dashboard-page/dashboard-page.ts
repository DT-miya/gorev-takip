import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DashboardService } from '../../../core/services/dashboard.service';
import { AssignedTask } from '../../../core/models/dashboard.model';


@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.css',
})
export class DashboardPage implements OnInit {
  tasks = signal<AssignedTask[]>([]);
  loading = signal(true);


  constructor(
    private dashboardService: DashboardService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.dashboardService.getAssignedTasks().subscribe({
      next: (data) => {
        this.tasks.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Görevler yüklenemedi', err);
        this.loading.set(false);
      }
    });
  }

  openBoard(projectId: number): void {
    this.router.navigate(['/projects', projectId, 'board']);
  }
}
