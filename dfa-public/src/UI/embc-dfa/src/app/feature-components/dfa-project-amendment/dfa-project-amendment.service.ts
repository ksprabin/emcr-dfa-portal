import { EventEmitter, Injectable } from '@angular/core';
import { FullTimeOccupant, DfaApplicationMain, OtherContact, SecondaryApplicant } from 'src/app/core/model/dfa-application-main.model';
import { Observable } from 'rxjs';
import { ApplicationService } from 'src/app/core/api/services';
import { DFAApplicationMainMappingService } from '../dfa-application-main/dfa-application-main-mapping.service';
import { DFAApplicationMainDataService } from '../dfa-application-main/dfa-application-main-data.service';

@Injectable({ providedIn: 'root' })
export class DFAApplicationMainService {
  private _fullTimeOccupants: Array<FullTimeOccupant> = [];
  private _otherContacts: Array<OtherContact> = [];
  private _secondaryApplicants: Array<SecondaryApplicant> = [];

  constructor(
    private dfaApplicationMainMapping: DFAApplicationMainMappingService,
    private dfaApplicationMainDataService: DFAApplicationMainDataService,
    private applicationService: ApplicationService
  ) {}

  public upsertProject(updatedProject: DfaApplicationMain): Observable<string> {
    return this.applicationService.applicationUpdateApplication({ body: updatedProject });
  }
}
