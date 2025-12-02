import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AuthService } from '../../../../core/services/auth.service';

interface Feature {
  icon: string;
  title: string;
  description: string;
}

interface Plan {
  name: string;
  price: string;
  period: string;
  features: string[];
  highlighted: boolean;
  color: string;
}

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatToolbarModule
  ],
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss']
})
export class LandingComponent {
  features: Feature[] = [
    {
      icon: 'point_of_sale',
      title: 'Punto de Venta Rápido',
      description: 'Interface intuitiva para procesar ventas en segundos'
    },
    {
      icon: 'inventory',
      title: 'Control de Inventario',
      description: 'Gestiona tu stock en tiempo real con alertas automáticas'
    },
    {
      icon: 'analytics',
      title: 'Reportes Detallados',
      description: 'Analiza tus ventas y toma decisiones informadas'
    },
    {
      icon: 'people',
      title: 'Multi-Usuario',
      description: 'Roles y permisos personalizados para tu equipo'
    },
    {
      icon: 'cloud',
      title: '100% en la Nube',
      description: 'Accede desde cualquier lugar, en cualquier momento'
    },
    {
      icon: 'security',
      title: 'Seguro y Confiable',
      description: 'Tus datos protegidos con encriptación de nivel empresarial'
    }
  ];

  plans: Plan[] = [
    {
      name: 'Básico',
      price: 'Gratis',
      period: '30 días de prueba',
      highlighted: false,
      color: '#0f766e',
      features: [
        '1 Usuario',
        '100 Productos',
        'Ventas ilimitadas',
        'Reportes básicos',
        'Soporte por email'
      ]
    },
    {
      name: 'Profesional',
      price: '$29',
      period: 'por mes',
      highlighted: true,
      color: '#14b8a6',
      features: [
        '5 Usuarios',
        'Productos ilimitados',
        'Ventas ilimitadas',
        'Reportes avanzados',
        'Soporte prioritario',
        'Multi-sucursal',
        'Facturación electrónica'
      ]
    },
    {
      name: 'Empresarial',
      price: '$79',
      period: 'por mes',
      highlighted: false,
      color: '#1e3a8a',
      features: [
        'Usuarios ilimitados',
        'Todo lo de Profesional',
        'API personalizada',
        'Soporte 24/7',
        'Capacitación incluida',
        'Servidor dedicado'
      ]
    }
  ];

  constructor(
    private router: Router,
    private authService: AuthService
  ) {
    // Si ya está autenticado, redirigir al dashboard
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
    }
  }

  navigateToLogin(): void {
    this.router.navigate(['/auth/login']);
  }

  navigateToRegister(): void {
    this.router.navigate(['/auth/register']);
  }

  scrollToSection(sectionId: string): void {
    const element = document.getElementById(sectionId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }
}

