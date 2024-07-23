import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DFAApplicationRoutingModule } from './dfa-application-routing.module';
import { DfaApplicationComponent } from './dfa-application.component';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatIconModule } from '@angular/material/icon'
import { CoreModule } from 'src/app/core/core.module';

@NgModule({
  declarations: [DfaApplicationComponent],
  imports: [CommonModule, DFAApplicationRoutingModule, MatButtonModule, MatIconModule, CoreModule]
})
export class DFADashApplicationModule { }
