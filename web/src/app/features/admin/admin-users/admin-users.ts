import { ChangeDetectorRef, Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { AdminService } from '@services/admin.service';
import { AdminUser } from '@models/admin.model';


@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-users.html',
  styleUrl: './admin-users.css'
})
export class AdminUsersComponent implements OnInit {
  users: AdminUser[] = [];
  
  // Sayfalandırma Değişkenleri
  currentPage = 1;
  pageSize = 20;
  totalCount = 0;
  totalPages = 1;
  
  // Filtreleme
  searchEmail = '';
  searchName = '';
  private searchSubject = new Subject<string>();
  isLoading = false;

private adminService = inject(AdminService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.loadUsers();

    // Arama input'una debounce (gecikme) ekliyoruz
    this.searchSubject.pipe(
      debounceTime(300),          // Kullanıcı yazmayı bıraktıktan 300ms sonra tetiklenir
      distinctUntilChanged()      // Aynı değer girildiyse tekrar istek atmaz
    ).subscribe(search => {
      this.searchEmail = this.searchEmail?.trim();  // Arama terimini güncelle
      this.searchName = this.searchName?.trim();  // Arama terimini güncelle
      this.currentPage = 1;      // Yeni aramada 1. sayfaya dön
      this.loadUsers();
    });
  }

  toggleRole(user: AdminUser): void {
    const newRole = user.role === 'Admin' ? 'User' : 'Admin';
    this.adminService.updateUserRole(user.id, newRole).subscribe({
      next: (updatedUser: AdminUser) => {
      // 1. Dizideki ilgili kullanıcının verilerini lokal olarak güncelle
      this.users = this.users.map(u => 
        u.id === updatedUser.id ? { ...u, role: updatedUser.role } : u
      );
      this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Rol değiştirilemedi', err);
       
      }
    });
  }

onSearchChange(searchValue: string): void {
    this.searchSubject.next(searchValue);
  }


onEmailChange(value: string): void {
  this.searchEmail = value;
  this.onSearchChange(this.searchEmail);
}

onNameChange(value: string): void {
  this.searchName = value;
  this.onSearchChange(this.searchName);
}



  loadUsers(): void {


  console.log('Arama Parametreleri:', { 
    email: this.searchEmail, 
    name: this.searchName 
  });
    this.isLoading = true;
    this.adminService.getUsers(this.searchEmail, this.searchName, this.currentPage, this.pageSize).subscribe({
      next: (result) => {
        this.users = result.items;
        this.totalCount = result.totalCount;
        this.totalPages = result.totalPages ?? 1;
        this.isLoading = false;

        // Arayüzün güncellendiğinden emin oluyoruz
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Kullanıcılar yüklenemedi:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadUsers();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadUsers();
    }
  }





}