import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  errorMessage: string | null = null;
  isLoading = false;

  ngOnInit(): void {
    const reason = this.route.snapshot.queryParamMap.get('reason');
    if (reason === 'session_expired') {
      this.errorMessage = 'Your session timed out. Please sign in again.';
    }
  }

  get emailControl() {
    return this.loginForm.get('email');
  }

  get passwordControl() {
    return this.loginForm.get('password');
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        console.log("ih");
        this.isLoading = false;
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        console.log("hi");
        this.isLoading = false;
        if (err.status === 401 && err.error?.message) {
          this.errorMessage = err.error.message;
        } else if (err.status === 429 && err.error?.message) {
          this.errorMessage = err.error.message;
        } else {
          this.errorMessage = 'Your email or password is incorrect.';
        }
      }
    });
  }
}
