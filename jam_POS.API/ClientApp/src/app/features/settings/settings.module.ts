import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SettingsComponent } from './components/settings/settings.component';
import { CategoryListComponent } from '../categories/components/category-list/category-list.component';

const routes: Routes = [
  {
    path: '',
    component: SettingsComponent
  },
  {
    path: 'categorias',
    component: CategoryListComponent
  }
];

@NgModule({
  imports: [
    SettingsComponent,
    CategoryListComponent,
    RouterModule.forChild(routes)
  ]
})
export class SettingsModule { }

