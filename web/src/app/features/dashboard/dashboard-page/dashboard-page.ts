import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DashboardService } from '../../../core/services/dashboard.service';
import { AssignedTask } from '../../../core/models/dashboard.model';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';


@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, RouterLink, MatButtonModule],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.css',
})
export class DashboardPage implements OnInit {
  tasks = signal<AssignedTask[]>([]);
  loading = signal(true);
  
  selectedPriority = signal<string>("Tümü");

  filteredTasks = computed(() => {
    const p = this.selectedPriority();
    const all = this.tasks();
    return p === "Tümü" ? all : all.filter(t => t.priority === p);
  });

  setPriorityFilter(priority: string): void {
    this.selectedPriority.set(priority);
  }




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
