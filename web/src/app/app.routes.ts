import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { TaskboardComponent } from './features/taskboard/taskboard.component';
import { authGuard } from './core/guards/auth.guard';
import { BoardViewComponent } from './features/board/pages/board-view.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { 
    path: 'taskboard', 
    component: TaskboardComponent, 
    canActivate: [authGuard] // Guard burada devreye giriyor
  },

  { path: 'projects/:id/board', 
    component: BoardViewComponent,
    canActivate: [authGuard] // Guard burada devreye giriyor
  
  }

];