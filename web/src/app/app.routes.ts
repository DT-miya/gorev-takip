import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { TaskboardComponent } from './features/taskboard/taskboard.component';
import { ProjectListComponent } from './features/projects/project-list/project-list';
import { authGuard } from './core/guards/auth.guard';
import { BoardViewComponent } from './features/board/pages/board-view.component';
import { DashboardPage } from './features/dashboard/dashboard-page/dashboard-page';

import { AdminStatsComponent } from './features/admin/admin-stats/admin-stats';
import { AdminUsersComponent } from './features/admin/admin-users/admin-users';

import { adminGuard } from '@core/guards/admin.guard';


export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  { 
    path: 'taskboard', 
    component: TaskboardComponent, 
    canActivate: [authGuard] // Guard burada devreye giriyor
  },

  {
    path: 'projects/:id/board', 
    component: BoardViewComponent,
    canActivate: [authGuard] // Guard burada devreye giriyor
  },
  
  {
    path: 'projects',
    component: ProjectListComponent,
    canActivate: [authGuard]
  },

  {
  path: 'dashboard',
  component: DashboardPage,
  canActivate: [authGuard]

  },
  
  {
  path: 'admin-test',
  component: AdminStatsComponent,
  canActivate: [authGuard]
},
{
  path: 'admin-users-test',
  component: AdminUsersComponent,
  canActivate: [authGuard]
},

  

  {
    path: 'admin',
    loadComponent: () => import('@features/admin/admin-dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent),
    canActivate: [adminGuard]
  },

  {
    path: 'admin-logs',
    loadComponent: () => import('@features/admin/admin-logs/admin-logs').then(m => m.AdminLogsComponent),
    canActivate: [adminGuard]
  }


];