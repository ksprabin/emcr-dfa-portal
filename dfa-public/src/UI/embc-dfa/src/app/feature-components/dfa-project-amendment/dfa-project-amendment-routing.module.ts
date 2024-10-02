import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DFAProjectAmendmentComponent } from './dfa-project-amendment.component';

const routes: Routes = [{ path: '', component: DFAProjectAmendmentComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DFAProjectAmendmentRoutingModule {}
