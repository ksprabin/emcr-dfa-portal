import { EventEmitter, Injectable } from '@angular/core';
import { CacheService } from 'src/app/core/services/cache.service';
import { ApplicationService, AttachmentService } from 'src/app/core/api/services';
import { BehaviorSubject, Observable } from 'rxjs';
import { DfaProjectMain, FileUpload, RecoveryPlan } from '../../core/model/dfa-project-main.model';

@Injectable({ providedIn: 'root' })
export class DFAProjectMainDataService {
  private _recoveryPlan: RecoveryPlan;
  private _fileUploads = [];
  private _dfaProjectMain: DfaProjectMain;
  private _isSubmitted: boolean = false;
  private _applicationId: string;
  private _projectId: string;
  private _claimId: string;
  private _appUrl: string;
  private _vieworedit: string;
  private _stepselected: string;
  private _isdisabled: string;
  private _editstep: string;
  private _requiredDocuments = [];
  public changeViewOrEdit: EventEmitter<string> = new EventEmitter<string>();
  public changeAppUrl: EventEmitter<string> = new EventEmitter<string>();
  public changeDisableFileUpload: EventEmitter<string> = new EventEmitter<string>();
  public stepSelected: EventEmitter<string> = new EventEmitter<string>();
  public changeProjectId: EventEmitter<string> = new EventEmitter<string>();

  constructor(
    private cacheService: CacheService,
    private fileUploadsService: AttachmentService,
    private applicationService: ApplicationService
  ) {
  }

  public get recoveryPlan(): RecoveryPlan {
    return this._recoveryPlan;
  }

  public set recoveryPlan(value: RecoveryPlan) {
    this._recoveryPlan = value;
  }


  public get requiredDocuments(): Array<string> {
    return this._requiredDocuments;
  }

  public set requiredDocuments(value: Array<string>) {
    this._requiredDocuments = value;
  }

  public get fileUploads(): Array<FileUpload> {
    return this._fileUploads;
  }
  public set fileUploads(value: Array<FileUpload>) {
    this._fileUploads = value;
  }

  public getDFAProjectMain(): DfaProjectMain {
    if (this._dfaProjectMain === null || undefined) {
      this._dfaProjectMain = JSON.parse(this.cacheService.get('dfa-project-main'));
    }
    return this._dfaProjectMain;
  }

  public get isSubmitted(): boolean {
    return this._isSubmitted;
  }
  public set isSubmitted(value: boolean) {
    this._isSubmitted = value;
  }

  public setDFAProjectMain(dfaProjectMain: DfaProjectMain): void {
    this._dfaProjectMain = dfaProjectMain;
    this.cacheService.set('dfa-project-main', dfaProjectMain);
  }

  public setApplicationId(applicationId: string): void {
    this._applicationId = applicationId;
    this.cacheService.set('applicationId', applicationId);
  }

  public getApplicationId(): string {
    if (this._applicationId === null || this._applicationId === undefined) {
      this._applicationId = this.cacheService.get('applicationId');
    }

    return this._applicationId;
  }

  public setProjectId(projectId: string): void {
    this._projectId = projectId;
    this.cacheService.set('projectId', projectId);
    this.changeProjectId.emit(projectId);
  }

  public getProjectId(): string {
    if (this._projectId === null || this._projectId === undefined) {
      this._projectId = this.cacheService.get('projectId');
    }

    return this._projectId;
  }

  public setClaimId(claimId: string): void {
    this._claimId = claimId;
    this.cacheService.set('claimId', claimId);
  }

  public getClaimId(): string {
    if (this._claimId === null || this._claimId === undefined) {
      this._claimId = this.cacheService.get('claimId');
    }

    return this._claimId;
  }

  public setViewOrEdit(vieworedit: string): void {
    this._vieworedit = vieworedit;
    this.changeViewOrEdit.emit(vieworedit);
    this.cacheService.set('vieworedit', vieworedit);
  }
  public getViewOrEdit(): string {
    if (this._vieworedit === null || this._vieworedit === undefined) {
      this._vieworedit = this.cacheService.get('vieworedit');
    }

    return this._vieworedit;
  }

  public setCurrentStepSelected(step: string): void {
    this._stepselected = step;
    this.stepSelected.emit(step);
    this.cacheService.set('step', step);
  }
  public getCurrentStepSelected(): string {
    if (this._stepselected === null || this._stepselected === undefined) {
      this._stepselected = this.cacheService.get('step');
    }

    return this._stepselected;
  }

  public setDisableFileUpload(isdisabled: string): void {
    this._isdisabled = isdisabled;
    this.changeDisableFileUpload.emit(isdisabled);
  }
  public getDisableFileUpload(): string {
    return this._isdisabled;
  }

  public setEditStep(editstep: string): void {
    this._editstep = editstep;
  }
  public getEditStep(): string {
    return this._editstep;
  }

   public createDFAProjectMainDTO(): DfaProjectMain {
    return {
      id: this._projectId,
      applicationId: this._applicationId,
      project: this._recoveryPlan
    };
  }
}
