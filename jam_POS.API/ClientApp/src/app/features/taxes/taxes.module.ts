import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TaxListComponent } from './components/tax-list/tax-list.component';

const routes: Routes = [
  {
    path: '',
    component: TaxListComponent
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes),
    TaxListComponent // Import standalone component
  ]
})
export class TaxesModule { }

