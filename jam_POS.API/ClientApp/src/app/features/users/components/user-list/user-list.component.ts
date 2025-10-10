import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatDividerModule } from '@angular/material/divider';
import { UserService } from '../../services/user.service';
import { RoleService } from '../../../roles/services/role.service';
import { User, UserFilter } from '../../models/user.model';
import { Role } from '../../../roles/models/role.model';
import { PagedResult } from '../../../../core/models/pagination.model';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatTooltipModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatPaginatorModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatChipsModule,
    MatSortModule,
    MatDividerModule
  ],
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {
  users: User[] = [];
  pagedResult: PagedResult<User> | null = null;
  userForm: FormGroup;
  filterForm: FormGroup;
  roles: Role[] = [];
  
  isEditing = false;
  editingId: number | null = null;
  showForm = false;
  loading = false;
  displayedColumns: string[] = ['usuario', 'nombre', 'email', 'rol', 'ultimoAcceso', 'estado', 'acciones'];

  pageSize = 10;
  pageNumber = 1;
  pageSizeOptions = [5, 10, 25, 50];
  sortBy = 'username';
  sortDescending = false;

  constructor(
    private userService: UserService,
    private roleService: RoleService,
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {
    this.userForm = this.fb.group({
      username: ['', [Validators.required, Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      roleId: [null, [Validators.required]],
      isActive: [true]
    });

    this.filterForm = this.fb.group({
      searchTerm: [''],
      roleId: [null],
      isActive: [null]
    });
  }

  ngOnInit(): void {
    this.loadUsers();
    this.loadRoles();

    this.filterForm.valueChanges.subscribe(() => {
      this.pageNumber = 1;
      this.loadUsers();
    });
  }

  loadUsers(): void {
    this.loading = true;
    const filter: UserFilter = {
      ...this.filterForm.value,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      orderBy: this.sortBy,
      orderDescending: this.sortDescending
    };

    this.userService.getUsers(filter).subscribe({
      next: (result) => {
        this.pagedResult = result;
        this.users = result.items;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error al cargar usuarios:', error);
        this.snackBar.open('Error al cargar los usuarios', 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  loadRoles(): void {
    this.roleService.getActiveRoles().subscribe({
      next: (roles) => {
        this.roles = roles;
      },
      error: (error) => {
        console.error('Error al cargar roles:', error);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadUsers();
  }

  onSortChange(sort: Sort): void {
    this.sortBy = sort.active || 'username';
    this.sortDescending = sort.direction === 'desc';
    this.loadUsers();
  }

  clearFilters(): void {
    this.filterForm.reset({
      searchTerm: '',
      roleId: null,
      isActive: null
    });
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    if (!this.showForm) {
      this.resetForm();
    }
  }

  onSubmit(): void {
    if (this.userForm.valid) {
      const userData = this.userForm.value;
      
      if (this.isEditing && this.editingId) {
        this.updateUser(userData);
      } else {
        this.createUser(userData);
      }
    } else {
      this.snackBar.open('Por favor complete todos los campos requeridos', 'Cerrar', { duration: 3000 });
    }
  }

  private createUser(userData: any): void {
    this.loading = true;
    this.userService.createUser(userData).subscribe({
      next: () => {
        this.loadUsers();
        this.resetForm();
        this.showForm = false;
        this.snackBar.open('Usuario creado exitosamente', 'Cerrar', { duration: 3000 });
        this.loading = false;
      },
      error: (error) => {
        const errorMsg = error.error?.message || 'Error al crear el usuario';
        this.snackBar.open(errorMsg, 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  private updateUser(userData: any): void {
    if (!this.editingId) return;
    
    const updateData = { ...userData, id: this.editingId };
    
    // Si no se cambió la contraseña, no enviarla
    if (!updateData.password) {
      delete updateData.password;
    }
    
    this.loading = true;
    this.userService.updateUser(this.editingId, updateData).subscribe({
      next: () => {
        this.loadUsers();
        this.resetForm();
        this.showForm = false;
        this.snackBar.open('Usuario actualizado exitosamente', 'Cerrar', { duration: 3000 });
        this.loading = false;
      },
      error: (error) => {
        const errorMsg = error.error?.message || 'Error al actualizar el usuario';
        this.snackBar.open(errorMsg, 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  editUser(user: User): void {
    this.isEditing = true;
    this.editingId = user.id;
    this.showForm = true;
    
    // Al editar, la contraseña no es requerida
    this.userForm.get('password')?.clearValidators();
    this.userForm.get('password')?.updateValueAndValidity();
    
    this.userForm.patchValue({
      username: user.username,
      email: user.email,
      password: '',
      firstName: user.firstName,
      lastName: user.lastName,
      roleId: user.roleId,
      isActive: user.isActive
    });

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  deleteUser(id: number, username: string): void {
    if (confirm(`¿Está seguro de que desea eliminar el usuario "${username}"?`)) {
      this.loading = true;
      this.userService.deleteUser(id).subscribe({
        next: () => {
          this.loadUsers();
          this.snackBar.open('Usuario eliminado exitosamente', 'Cerrar', { duration: 3000 });
          this.loading = false;
        },
        error: (error) => {
          const errorMsg = error.error?.message || 'Error al eliminar el usuario';
          this.snackBar.open(errorMsg, 'Cerrar', { duration: 5000 });
          this.loading = false;
        }
      });
    }
  }

  resetForm(): void {
    this.userForm.reset({ 
      isActive: true 
    });
    
    // Restaurar validación de contraseña para modo crear
    this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.userForm.get('password')?.updateValueAndValidity();
    
    this.isEditing = false;
    this.editingId = null;
  }

  cancelEdit(): void {
    this.resetForm();
    this.showForm = false;
  }

  getActiveFilterCount(): number {
    const filters = this.filterForm.value;
    let count = 0;
    if (filters.searchTerm) count++;
    if (filters.roleId !== null) count++;
    if (filters.isActive !== null) count++;
    return count;
  }

  formatDate(date?: Date | string): string {
    if (!date) return 'Nunca';
    return new Date(date).toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}

