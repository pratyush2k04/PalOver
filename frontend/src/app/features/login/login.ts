import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '@shared/services/auth';
import { AppContextService } from '@shared/services/app-context';
import { AlertComponent } from '@common/components/alert/alert';
import { ButtonComponent } from '@common/components/button/button';
import { InputComponent } from '@common/components/form/input/input';
import { LoaderService } from '@shared/services/loader.service';
import { GlobalLoader } from '@common/components/global-loader/global-loader';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AlertComponent, ButtonComponent, InputComponent, GlobalLoader],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private appContext = inject(AppContextService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private loaderService = inject(LoaderService);

  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    const reason = this.route.snapshot.queryParamMap.get('reason');
    if (reason === 'session_expired') {
      this.errorMessage.set('Your session timed out. Please sign in again.');
    }
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.loaderService.show();
    this.errorMessage.set(null);

    this.authService.login(this.loginForm.value).subscribe({
      next: async () => {
        const context = await this.appContext.initialize();
        await this.router.navigateByUrl(context.workspace.route);
        this.loaderService.hide();
      },
      error: (err) => {
        this.loaderService.hide();
        if (err.status === 401 && err.error?.message) {
          this.errorMessage.set(err.error.message);
        } else if (err.status === 429 && err.error?.message) {
          this.errorMessage.set(err.error.message);
        } else {
          this.errorMessage.set('Your email or password is incorrect.');
        }
      }
    });
  }
}
