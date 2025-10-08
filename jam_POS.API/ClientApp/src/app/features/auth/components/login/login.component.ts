import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../../core/services/auth.service';
import { LoginRequest } from '../../../../core/models/user.model';
import { APP_CONSTANTS } from '../../../../core/constants/app-constants.constant';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  isLoading = false;
  hidePassword = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.loginForm = this.fb.group({
      username: ['', [
        Validators.required, 
        Validators.minLength(APP_CONSTANTS.VALIDATION.USERNAME_MIN_LENGTH)
      ]],
      password: ['', [
        Validators.required, 
        Validators.minLength(APP_CONSTANTS.VALIDATION.PASSWORD_MIN_LENGTH)
      ]]
    });
  }

  ngOnInit(): void {
    // Redirect if already authenticated
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
    }
  }

  onSubmit(): void {
    if (this.loginForm.valid && !this.isLoading) {
      this.isLoading = true;
      
      const loginRequest: LoginRequest = {
        username: this.loginForm.value.username,
        password: this.loginForm.value.password
      };

      this.authService.login(loginRequest).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.snackBar.open('¡Inicio de sesión exitoso!', 'Cerrar', {
            duration: 3000,
            horizontalPosition: 'center',
            verticalPosition: 'top'
          });
          
          // Navigate to dashboard
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          this.isLoading = false;
          console.error('Login error:', error);
          
          let errorMessage = 'Error al iniciar sesión. Por favor, intente nuevamente.';
          if (error.error?.message) {
            errorMessage = error.error.message;
          }
          
          this.snackBar.open(errorMessage, 'Cerrar', {
            duration: 5000,
            horizontalPosition: 'center',
            verticalPosition: 'top',
            panelClass: ['error-snackbar']
          });
        }
      });
    }
  }

  togglePasswordVisibility(): void {
    this.hidePassword = !this.hidePassword;
  }

  getErrorMessage(fieldName: string): string {
    const field = this.loginForm.get(fieldName);
    if (field?.hasError('required')) {
      return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} es requerido`;
    }
    if (field?.hasError('minlength')) {
      const minLength = field.errors?.['minlength']?.requiredLength;
      return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} debe tener al menos ${minLength} caracteres`;
    }
    return '';
  }
}
