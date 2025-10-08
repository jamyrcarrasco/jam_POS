import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService, User } from '../services/auth.service';

interface DashboardCard {
  title: string;
  description: string;
  icon: string;
  route: string;
  color: string;
  roles: string[];
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatGridListModule,
    MatToolbarModule,
    MatMenuModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  currentUser: User | null = null;
  dashboardCards: DashboardCard[] = [];

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Check authentication
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    // Get current user
    this.currentUser = this.authService.getCurrentUser();
    
    // Initialize dashboard cards based on user role
    this.initializeDashboardCards();
  }

  private initializeDashboardCards(): void {
    const allCards: DashboardCard[] = [
      {
        title: 'Productos',
        description: 'Gestionar inventario de productos',
        icon: 'inventory',
        route: '/productos',
        color: 'primary',
        roles: ['SuperAdmin', 'Seller']
      },
      {
        title: 'Ventas',
        description: 'Procesar ventas y transacciones',
        icon: 'point_of_sale',
        route: '/ventas',
        color: 'accent',
        roles: ['SuperAdmin', 'Seller']
      },
      {
        title: 'Clientes',
        description: 'Administrar base de datos de clientes',
        icon: 'people',
        route: '/clientes',
        color: 'warn',
        roles: ['SuperAdmin', 'Seller']
      },
      {
        title: 'Reportes',
        description: 'Generar reportes y estadísticas',
        icon: 'analytics',
        route: '/reportes',
        color: 'primary',
        roles: ['SuperAdmin']
      },
      {
        title: 'Usuarios',
        description: 'Gestionar usuarios y roles',
        icon: 'admin_panel_settings',
        route: '/usuarios',
        color: 'accent',
        roles: ['SuperAdmin']
      },
      {
        title: 'Configuración',
        description: 'Configurar sistema y parámetros',
        icon: 'settings',
        route: '/configuracion',
        color: 'warn',
        roles: ['SuperAdmin']
      }
    ];

    // Filter cards based on user role
    if (this.currentUser) {
      this.dashboardCards = allCards.filter(card => 
        card.roles.includes(this.currentUser!.role)
      );
    }
  }

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }

  logout(): void {
    this.authService.logout();
  }

  getUserDisplayName(): string {
    if (this.currentUser) {
      return `${this.currentUser.firstName} ${this.currentUser.lastName}`.trim() || this.currentUser.username;
    }
    return 'Usuario';
  }

  getUserRoleDisplay(): string {
    if (this.currentUser) {
      switch (this.currentUser.role) {
        case 'SuperAdmin':
          return 'Super Administrador';
        case 'Seller':
          return 'Vendedor';
        default:
          return this.currentUser.role;
      }
    }
    return '';
  }
}
