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
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { RoleService } from '../../services/role.service';
import { Role, Permission, PermissionsByModule } from '../../models/role.model';

@Component({
  selector: 'app-role-list',
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
    MatSlideToggleModule,
    MatChipsModule,
    MatDividerModule,
    MatExpansionModule,
    MatCheckboxModule
  ],
  templateUrl: './role-list.component.html',
  styleUrls: ['./role-list.component.scss']
})
export class RoleListComponent implements OnInit {
  roles: Role[] = [];
  permissionsByModule: PermissionsByModule = {};
  roleForm: FormGroup;
  selectedPermissions: Set<number> = new Set();
  
  isEditing = false;
  editingId: number | null = null;
  showForm = false;
  loading = false;
  displayedColumns: string[] = ['nombre', 'descripcion', 'usuarios', 'permisos', 'activo', 'acciones'];

  constructor(
    private roleService: RoleService,
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {
    this.roleForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(50)]],
      description: ['', [Validators.required, Validators.maxLength(200)]],
      activo: [true],
      permissionIds: [[]]
    });
  }

  ngOnInit(): void {
    this.loadRoles();
    this.loadPermissions();
  }

  loadRoles(): void {
    this.loading = true;
    this.roleService.getRoles(true).subscribe({
      next: (roles) => {
        this.roles = roles;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error al cargar roles:', error);
        this.snackBar.open('Error al cargar los roles', 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  loadPermissions(): void {
    this.roleService.getPermissionsByModule().subscribe({
      next: (permissions) => {
        this.permissionsByModule = permissions;
      },
      error: (error) => {
        console.error('Error al cargar permisos:', error);
      }
    });
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    if (!this.showForm) {
      this.resetForm();
    }
  }

  onSubmit(): void {
    if (this.roleForm.valid) {
      const roleData = {
        ...this.roleForm.value,
        permissionIds: Array.from(this.selectedPermissions)
      };
      
      if (this.isEditing && this.editingId) {
        this.updateRole(roleData);
      } else {
        this.createRole(roleData);
      }
    }
  }

  private createRole(roleData: any): void {
    this.loading = true;
    this.roleService.createRole(roleData).subscribe({
      next: () => {
        this.loadRoles();
        this.resetForm();
        this.showForm = false;
        this.snackBar.open('Rol creado exitosamente', 'Cerrar', { duration: 3000 });
        this.loading = false;
      },
      error: (error) => {
        const errorMsg = error.error?.message || 'Error al crear el rol';
        this.snackBar.open(errorMsg, 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  private updateRole(roleData: any): void {
    if (!this.editingId) return;
    
    this.loading = true;
    this.roleService.updateRole(this.editingId, roleData).subscribe({
      next: () => {
        this.loadRoles();
        this.resetForm();
        this.showForm = false;
        this.snackBar.open('Rol actualizado exitosamente', 'Cerrar', { duration: 3000 });
        this.loading = false;
      },
      error: (error) => {
        const errorMsg = error.error?.message || 'Error al actualizar el rol';
        this.snackBar.open(errorMsg, 'Cerrar', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  editRole(role: Role): void {
    this.isEditing = true;
    this.editingId = role.id;
    this.showForm = true;
    this.roleForm.patchValue({
      name: role.name,
      description: role.description,
      activo: role.activo
    });

    // Cargar permisos seleccionados
    this.selectedPermissions.clear();
    if (role.permissions) {
      role.permissions.forEach(p => this.selectedPermissions.add(p.id));
    }

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  deleteRole(id: number): void {
    if (confirm('¿Está seguro de que desea eliminar este rol?')) {
      this.loading = true;
      this.roleService.deleteRole(id).subscribe({
        next: () => {
          this.loadRoles();
          this.snackBar.open('Rol eliminado exitosamente', 'Cerrar', { duration: 3000 });
          this.loading = false;
        },
        error: (error) => {
          const errorMsg = error.error?.message || 'Error al eliminar el rol';
          this.snackBar.open(errorMsg, 'Cerrar', { duration: 5000 });
          this.loading = false;
        }
      });
    }
  }

  togglePermission(permissionId: number): void {
    if (this.selectedPermissions.has(permissionId)) {
      this.selectedPermissions.delete(permissionId);
    } else {
      this.selectedPermissions.add(permissionId);
    }
  }

  isPermissionSelected(permissionId: number): boolean {
    return this.selectedPermissions.has(permissionId);
  }

  selectAllInModule(module: string): void {
    const permissions = this.permissionsByModule[module] || [];
    permissions.forEach(p => this.selectedPermissions.add(p.id));
  }

  deselectAllInModule(module: string): void {
    const permissions = this.permissionsByModule[module] || [];
    permissions.forEach(p => this.selectedPermissions.delete(p.id));
  }

  getModuleSelectedCount(module: string): number {
    const permissions = this.permissionsByModule[module] || [];
    return permissions.filter(p => this.selectedPermissions.has(p.id)).length;
  }

  resetForm(): void {
    this.roleForm.reset({ activo: true });
    this.selectedPermissions.clear();
    this.isEditing = false;
    this.editingId = null;
  }

  cancelEdit(): void {
    this.resetForm();
    this.showForm = false;
  }

  getModules(): string[] {
    return Object.keys(this.permissionsByModule);
  }
}

