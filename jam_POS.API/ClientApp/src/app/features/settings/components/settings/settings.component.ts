import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { filter } from 'rxjs/operators';

interface SettingCard {
  title: string;
  description: string;
  icon: string;
  route: string;
  color: string;
  available: boolean;
}

interface Breadcrumb {
  label: string;
  url: string;
}

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatDividerModule,
    MatChipsModule
  ],
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent {
  breadcrumbs: Breadcrumb[] = [];

  settingCards: SettingCard[] = [
    {
      title: 'Categorías',
      description: 'Gestiona las categorías de productos',
      icon: 'category',
      route: '/configuraciones/categorias',
      color: '#3B82F6',
      available: true
    },
    {
      title: 'Usuarios',
      description: 'Administra usuarios y permisos del sistema',
      icon: 'people',
      route: '/configuraciones/usuarios',
      color: '#10B981',
      available: false
    },
    {
      title: 'Empresa',
      description: 'Configura la información de tu empresa',
      icon: 'business',
      route: '/configuraciones/empresa',
      color: '#8B5CF6',
      available: false
    },
    {
      title: 'Impuestos',
      description: 'Configura los impuestos y tasas',
      icon: 'receipt_long',
      route: '/configuraciones/impuestos',
      color: '#F59E0B',
      available: false
    },
    {
      title: 'General',
      description: 'Configuración general del sistema',
      icon: 'settings',
      route: '/configuraciones/general',
      color: '#EC4899',
      available: false
    },
    {
      title: 'Punto de Venta',
      description: 'Configura las opciones del punto de venta',
      icon: 'point_of_sale',
      route: '/configuraciones/pos',
      color: '#14B8A6',
      available: false
    }
  ];

  constructor(private router: Router) {
    this.updateBreadcrumbs();
  }

  navigateTo(route: string, available: boolean): void {
    if (available) {
      this.router.navigate([route]);
    }
  }

  private updateBreadcrumbs(): void {
    const url = this.router.url;
    this.breadcrumbs = [{ label: 'Configuraciones', url: '/configuraciones' }];

    if (url.includes('/categorias')) {
      this.breadcrumbs.push({ label: 'Categorías', url: '/configuraciones/categorias' });
    } else if (url.includes('/usuarios')) {
      this.breadcrumbs.push({ label: 'Usuarios', url: '/configuraciones/usuarios' });
    } else if (url.includes('/empresa')) {
      this.breadcrumbs.push({ label: 'Empresa', url: '/configuraciones/empresa' });
    } else if (url.includes('/impuestos')) {
      this.breadcrumbs.push({ label: 'Impuestos', url: '/configuraciones/impuestos' });
    } else if (url.includes('/general')) {
      this.breadcrumbs.push({ label: 'General', url: '/configuraciones/general' });
    } else if (url.includes('/pos')) {
      this.breadcrumbs.push({ label: 'Punto de Venta', url: '/configuraciones/pos' });
    }
  }
}

