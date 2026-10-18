import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { PageHeaderComponent } from '@common/components/page-header/page-header';
import { ToastService } from '@common/services/toast/toast';
import { PROFILE_MESSAGES } from '@shared/constants/app.constants';
import {
  hasLowerCase,
  hasNumber,
  hasSymbol,
  hasUpperCase,
  passwordStrengthValidator
} from '@shared/utils/utils';
import { AppContextService } from '@shared/services/app-context';
import { ProfileService } from './services/profile.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, AlertComponent, ButtonComponent, InputComponent, PageHeaderComponent],
  providers: [DatePipe],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class ProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private profileService = inject(ProfileService);
  private toastService = inject(ToastService);
  private appContext = inject(AppContextService);
  private datePipe = inject(DatePipe);

  readonly role = signal<string>('');
  readonly lastLoginAt = signal<string | null>(null);

  readonly lastLoginDisplay = computed(() => {
    const raw = this.lastLoginAt();
    if (!raw) return '—';
    return this.datePipe.transform(raw, 'medium') ?? '—';
  });

  readonly isLoadingProfile = signal(true);
  readonly isSavingProfile = signal(false);
  readonly isChangingPassword = signal(false);

  readonly profileErrorMessage = signal<string | null>(null);
  readonly passwordErrorMessage = signal<string | null>(null);
  readonly currentPasswordError = signal<string | null>(null);

  profileForm: FormGroup = this.fb.group({
    fullName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]]
  });

  passwordForm: FormGroup = this.fb.group({
    currentPassword: ['', [Validators.required]],
    newPassword: ['', [Validators.required, passwordStrengthValidator]]
  });

  private readonly newPasswordValue = signal('');

  readonly hasMinLength = computed(() => this.newPasswordValue().length >= 10);
  readonly hasUpper = computed(() => hasUpperCase(this.newPasswordValue()));
  readonly hasLower = computed(() => hasLowerCase(this.newPasswordValue()));
  readonly hasNumberChar = computed(() => hasNumber(this.newPasswordValue()));
  readonly hasSymbolChar = computed(() => hasSymbol(this.newPasswordValue()));

  ngOnInit(): void {
    this.passwordForm.get('newPassword')!.valueChanges.subscribe((value) => {
      this.newPasswordValue.set(value ?? '');
    });

    this.loadProfile();
  }

  private loadProfile(): void {
    this.isLoadingProfile.set(true);
    this.profileService.getMe().subscribe({
      next: (response) => {
        this.profileForm.patchValue({
          fullName: response.fullName,
          email: response.email
        });
        this.role.set(response.role);
        this.lastLoginAt.set(response.lastLoginAt);
        this.isLoadingProfile.set(false);
      },
      error: () => {
        this.profileErrorMessage.set(PROFILE_MESSAGES.LOAD_PROFILE_ERROR);
        this.isLoadingProfile.set(false);
      }
    });
  }

  onSaveProfile(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.isSavingProfile.set(true);
    this.profileErrorMessage.set(null);

    this.profileService.updateMe(this.profileForm.value).subscribe({
      next: async (response) => {
        this.isSavingProfile.set(false);
        this.profileForm.patchValue({ fullName: response.fullName, email: response.email }, { emitEvent: false });
        this.appContext.reset();
        await this.appContext.initialize();
        this.toastService.success(PROFILE_MESSAGES.PROFILE_UPDATED);
      },
      error: (err) => {
        this.isSavingProfile.set(false);
        this.profileErrorMessage.set(err.error?.message ?? PROFILE_MESSAGES.UPDATE_PROFILE_ERROR);
      }
    });
  }

  onChangePassword(): void {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    this.isChangingPassword.set(true);
    this.passwordErrorMessage.set(null);
    this.currentPasswordError.set(null);

    this.profileService.changePassword(this.passwordForm.value).subscribe({
      next: () => {
        this.isChangingPassword.set(false);
        this.passwordForm.reset();
        this.toastService.success(PROFILE_MESSAGES.PASSWORD_CHANGED);
      },
      error: (err) => {
        this.isChangingPassword.set(false);
        if (err.status === 400 && err.error?.message) {
          this.currentPasswordError.set(err.error.message);
        } else {
          this.passwordErrorMessage.set(PROFILE_MESSAGES.CHANGE_PASSWORD_ERROR);
        }
      }
    });
  }
}
