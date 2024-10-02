import { EventEmitter, Injectable } from '@angular/core';
import { CacheService } from 'src/app/core/services/cache.service';
import { DFAApplicationStartDataService } from '../dfa-application-start/dfa-application-start-data.service';
import { ApplicationService, AttachmentService } from 'src/app/core/api/services';

@Injectable({ providedIn: 'root' })
export class DFAProjectAmendmentDataService {
  private _applicationId: string;
  private _projectId: string;
  public changeViewOrEdit: EventEmitter<string> = new EventEmitter<string>();

  constructor(
    private cacheService: CacheService,
    private dfaApplicationStartDataService: DFAApplicationStartDataService,
    private fileUploadsService: AttachmentService,
    private applicationService: ApplicationService
  ) {
  }
  
}
