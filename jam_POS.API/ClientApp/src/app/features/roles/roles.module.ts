import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoleListComponent } from './components/role-list/role-list.component';

const routes: Routes = [
  {
    path: '',
    component: RoleListComponent
  }
];

@NgModule({
  imports: [
    RoleListComponent,
    RouterModule.forChild(routes)
  ]
})
export class RolesModule { }

