import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProfileService, UserProfile } from '@services/profile.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {
  profile: UserProfile | null = null;

  // Profil Form Modeli
  profileModel = { fullName: '', email: '' };
  
  // Şifre Form Modeli
  passwordModel = { currentPassword: '', newPassword: '', confirmPassword: '' };

  // Durum Değişkenleri
  isProfileLoading = false;
  isPasswordLoading = false;
  
  profileSuccessMsg = '';
  profileErrorMsg = '';

  passwordSuccessMsg = '';
  passwordErrorMsg = '';

  private profileService = inject(ProfileService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.profileService.getProfile().subscribe({
      next: (data) => {
        this.profile = data;
        this.profileModel.fullName = data.fullName;
        this.profileModel.email = data.email;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.profileErrorMsg = 'Profil bilgileri yüklenemedi.';
        this.cdr.detectChanges();
      }
    });
  }

  onUpdateProfile(): void {
    if (!this.profileModel.fullName || !this.profileModel.email) {
      this.profileErrorMsg = 'Lütfen tüm alanları doldurun.';
      this.cdr.detectChanges();
      return;
    }

    this.isProfileLoading = true;
    this.profileSuccessMsg = '';
    this.profileErrorMsg = '';
    this.cdr.detectChanges();

    this.profileService.updateProfile(this.profileModel).subscribe({
      next: (res) => {
        this.isProfileLoading = false;
        this.profile = res;
        this.profileSuccessMsg = 'Profil bilgileriniz başarıyla güncellendi.';
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isProfileLoading = false;
        this.profileErrorMsg = err.error?.message || 'Profil güncellenirken bir hata oluştu.';
        this.cdr.detectChanges();
      }
    });
  }

  onChangePassword(): void {
    if (!this.passwordModel.currentPassword || !this.passwordModel.newPassword) {
      this.passwordErrorMsg = 'Lütfen tüm alanları doldurun.';
      this.cdr.detectChanges();
      return;
    }

    if (this.passwordModel.newPassword !== this.passwordModel.confirmPassword) {
      this.passwordErrorMsg = 'Yeni şifreler eşleşmiyor.';
      this.cdr.detectChanges();
      return;
    }

    this.isPasswordLoading = true;
    this.passwordSuccessMsg = '';
    this.passwordErrorMsg = '';
    this.cdr.detectChanges();

    this.profileService.changePassword({
      currentPassword: this.passwordModel.currentPassword,
      newPassword: this.passwordModel.newPassword
    }).subscribe({
      next: (res) => {
        this.isPasswordLoading = false;
        this.passwordSuccessMsg = res.message || 'Şifreniz başarıyla güncellendi.';
        this.passwordModel = { currentPassword: '', newPassword: '', confirmPassword: '' };
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isPasswordLoading = false;
        this.passwordErrorMsg = err.error?.message || 'Şifre değiştirilirken bir hata oluştu.';
        this.cdr.detectChanges();
      }
    });
  }
}