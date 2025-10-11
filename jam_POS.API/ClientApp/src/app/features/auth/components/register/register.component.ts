import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatStepperModule } from '@angular/material/stepper';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatStepperModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  empresaFormGroup: FormGroup;
  adminFormGroup: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.empresaFormGroup = this.fb.group({
      nombreEmpresa: ['', [Validators.required, Validators.maxLength(200)]],
      nombreComercial: ['', [Validators.required, Validators.maxLength(100)]],
      rnc: ['', [Validators.maxLength(50)]],
      emailEmpresa: ['', [Validators.email, Validators.maxLength(100)]],
      telefono: ['', [Validators.maxLength(50)]]
    });

    this.adminFormGroup = this.fb.group({
      adminFirstName: ['', [Validators.required, Validators.maxLength(100)]],
      adminLastName: ['', [Validators.required, Validators.maxLength(100)]],
      adminEmail: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
      adminUsername: ['', [Validators.required, Validators.maxLength(50)]],
      adminPassword: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(100)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(group: FormGroup) {
    const password = group.get('adminPassword')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  onSubmit(): void {
    if (this.empresaFormGroup.valid && this.adminFormGroup.valid) {
      this.loading = true;

      const registerData = {
        ...this.empresaFormGroup.value,
        ...this.adminFormGroup.value
      };

      // Remove confirmPassword
      delete registerData.confirmPassword;

      this.http.post(`${environment.apiUrl}empresas/register`, registerData).subscribe({
        next: (response) => {
          this.snackBar.open(
            '¡Empresa registrada exitosamente! Ahora puedes iniciar sesión', 
            'Cerrar', 
            { duration: 5000 }
          );
          this.loading = false;
          this.router.navigate(['/auth/login']);
        },
        error: (error) => {
          const errorMsg = error.error?.message || 'Error al registrar la empresa';
          this.snackBar.open(errorMsg, 'Cerrar', { duration: 5000 });
          this.loading = false;
        }
      });
    }
  }

  navigateToLogin(): void {
    this.router.navigate(['/auth/login']);
  }
}

