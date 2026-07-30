import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { ProjectListComponent } from './features/projects/project-list/project-list';
import { authGuard } from './core/guards/auth.guard';
import { BoardViewComponent } from './features/board/pages/board-view.component';
import { DashboardPage } from './features/dashboard/dashboard-page/dashboard-page';

import { AdminStatsComponent } from './features/admin/admin-stats/admin-stats';
import { AdminUsersComponent } from './features/admin/admin-users/admin-users';

import { adminGuard } from '@core/guards/admin.guard';
import { AdminProjects } from '@features/admin-projects/admin-projects';
import { guestGuard } from '@core/guards/guest.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent, canActivate: [guestGuard]  },
  { path: 'register', component: RegisterComponent, canActivate: [guestGuard] },


  {
    path: 'projects/:id/board', 
    component: BoardViewComponent,
    canActivate: [authGuard] 
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
    path: 'profile',
    loadComponent: () => import('@features/profile/profile.component').then(m => m.ProfileComponent),
    canActivate: [authGuard]
  },
  
  {
    path: 'admin-stats',
    component: AdminStatsComponent,
    canActivate: [adminGuard]
  },
  {
    path: 'admin-users',
    component: AdminUsersComponent,
    canActivate: [adminGuard]
  },
  {
    path: 'admin-projects',
    component: AdminProjects,
    canActivate: [adminGuard]
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