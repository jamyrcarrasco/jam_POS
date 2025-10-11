import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SettingsComponent } from './components/settings/settings.component';
import { CategoryListComponent } from '../categories/components/category-list/category-list.component';
import { RoleListComponent } from '../roles/components/role-list/role-list.component';
import { UserListComponent } from '../users/components/user-list/user-list.component';
import { TaxListComponent } from '../taxes/components/tax-list/tax-list.component';

const routes: Routes = [
  {
    path: '',
    component: SettingsComponent
  },
  {
    path: 'categorias',
    component: CategoryListComponent
  },
  {
    path: 'roles',
    component: RoleListComponent
  },
  {
    path: 'usuarios',
    component: UserListComponent
  },
  {
    path: 'impuestos',
    component: TaxListComponent
  }
];

@NgModule({
  imports: [
    SettingsComponent,
    CategoryListComponent,
    RoleListComponent,
    UserListComponent,
    TaxListComponent,
    RouterModule.forChild(routes)
  ]
})
export class SettingsModule { }

