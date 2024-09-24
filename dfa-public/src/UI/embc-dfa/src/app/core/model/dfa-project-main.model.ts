import {
  UntypedFormBuilder,
  UntypedFormControl,
  UntypedFormGroup,
  Validators,
} from '@angular/forms';
import { Community, Country, StateProvince } from './address';
import { CustomValidationService } from '../services/customValidation.service';
import { SignatureBlock, SecondaryApplicantTypeOption, FileCategory, RoomType, RequiredDocumentType, ProjectStageOptionSet } from 'src/app/core/api/models';


export class RecoveryPlan {
  sitelocationdamageFromDate?: null | string;
  sitelocationdamageToDate?: null | string;
  projectNumber?: null | string;
  projectStatus?: null | ProjectStageOptionSet;
  projectName?: null | string;
  isdamagedDateSameAsApplication?: null | boolean;
  differentDamageDatesReason?: null | string;
  siteLocation?: null | string;
  infraDamageDetails?: null | string;
  causeofDamageDetails?: null | string;
  describeDamageDetails?: null | string;
  describeDamagedInfrastructure?: null | string;
  repairWorkDetails?: null | string;
  repairDamagedInfrastructure?: null | string;
  estimatedCompletionDate?: null | string;
  estimateCostIncludingTax?: null | number;

  constructor(
    sitelocationdamageFromDate?: null | string,
    sitelocationdamageToDate?: null | string,
    projectNumber?: null | string,
    projectStatus?: null | ProjectStageOptionSet,
    projectName?: null | string,
    isdamagedDateSameAsApplication?: null | boolean,
    differentDamageDatesReason?: null | string,
    siteLocation?: null | string,
    infraDamageDetails?: null | string,
    causeofDamageDetails?: null | string,
    describeDamageDetails?: null | string,
    describeDamagedInfrastructure?: null | string,
    repairWorkDetails?: null | string,
    repairDamagedInfrastructure?: null | string,
    estimatedCompletionDate?: null | string,
    estimateCostIncludingTax?: null | number
  ) { }
}

export class RecoveryPlanForm {
  sitelocationdamageFromDate = new UntypedFormControl();
  sitelocationdamageToDate = new UntypedFormControl();
  projectNumber = new UntypedFormControl();
  projectName = new UntypedFormControl();
  projectStatus = new UntypedFormControl();
  isdamagedDateSameAsApplication = new UntypedFormControl();
  differentDamageDatesReason = new UntypedFormControl();
  siteLocation = new UntypedFormControl();
  infraDamageDetails = new UntypedFormControl();
  causeofDamageDetails = new UntypedFormControl();
  describeDamageDetails = new UntypedFormControl();
  describeDamagedInfrastructure = new UntypedFormControl();
  repairWorkDetails = new UntypedFormControl();
  repairDamagedInfrastructure = new UntypedFormControl();
  estimatedCompletionDate = new UntypedFormControl();
  estimateCostIncludingTax = new UntypedFormControl();

  constructor(
    recoveryPlan: RecoveryPlan,
    customValidator: CustomValidationService
  ) {
    
    if (recoveryPlan.sitelocationdamageFromDate) {
      this.sitelocationdamageFromDate.setValue(recoveryPlan.sitelocationdamageFromDate);
    }
    this.sitelocationdamageFromDate.setValidators(null);

    if (recoveryPlan.sitelocationdamageToDate) {
      this.sitelocationdamageToDate.setValue(recoveryPlan.sitelocationdamageToDate);
    }
    this.sitelocationdamageToDate.setValidators(null);

    if (recoveryPlan.projectNumber) {
      this.projectNumber.setValue(recoveryPlan.projectNumber);
    }
    this.projectNumber.setValidators([customValidator
      .isRequired(this.projectNumber)
      .bind(customValidator)]);

    if (recoveryPlan.projectName) {
      this.projectName.setValue(recoveryPlan.projectName);
    }
    this.projectName.setValidators(null);

    if (recoveryPlan.projectStatus) {
      this.projectStatus.setValue(recoveryPlan.projectStatus);
    }
    this.projectStatus.setValidators(null);

    if (recoveryPlan.isdamagedDateSameAsApplication) {
      this.isdamagedDateSameAsApplication.setValue(recoveryPlan.isdamagedDateSameAsApplication);
    }
    this.isdamagedDateSameAsApplication.setValidators(null);

    if (recoveryPlan.differentDamageDatesReason) {
      this.differentDamageDatesReason.setValue(recoveryPlan.differentDamageDatesReason);
    }
    this.differentDamageDatesReason.setValidators(null);

    if (recoveryPlan.siteLocation) {
      this.siteLocation.setValue(recoveryPlan.siteLocation);
    }
    this.siteLocation.setValidators(null);

    if (recoveryPlan.infraDamageDetails) {
      this.infraDamageDetails.setValue(recoveryPlan.infraDamageDetails);
    }
    this.infraDamageDetails.setValidators(null);

    if (recoveryPlan.causeofDamageDetails) {
      this.causeofDamageDetails.setValue(recoveryPlan.causeofDamageDetails);
    }
    this.causeofDamageDetails.setValidators(null);

    if (recoveryPlan.describeDamageDetails) {
      this.describeDamageDetails.setValue(recoveryPlan.describeDamageDetails);
    }
    this.describeDamageDetails.setValidators(null);

    if (recoveryPlan.describeDamagedInfrastructure) {
      this.describeDamagedInfrastructure.setValue(recoveryPlan.describeDamagedInfrastructure);
    }
    this.describeDamagedInfrastructure.setValidators(null);

    if (recoveryPlan.repairWorkDetails) {
      this.repairWorkDetails.setValue(recoveryPlan.repairWorkDetails);
    }
    this.repairWorkDetails.setValidators(null);

    if (recoveryPlan.repairDamagedInfrastructure) {
      this.repairDamagedInfrastructure.setValue(recoveryPlan.repairDamagedInfrastructure);
    }
    this.repairDamagedInfrastructure.setValidators(null);

    if (recoveryPlan.estimatedCompletionDate) {
      this.estimatedCompletionDate.setValue(recoveryPlan.estimatedCompletionDate);
    }
    this.estimatedCompletionDate.setValidators(null);

    if (recoveryPlan.estimateCostIncludingTax) {
      this.estimateCostIncludingTax.setValue(recoveryPlan.estimateCostIncludingTax);
    }
    this.estimateCostIncludingTax.setValidators(null);
  }
}


export class FileUpload {
  projectId?: string;
  contentType?: string;
  deleteFlag?: boolean;
  fileData?: string;
  fileDescription?: string;
  fileName?: string;
  fileSize?: number;
  fileType?: FileCategory;
  fileTypeText?: string;
  requiredDocumentType?: RequiredDocumentType;
  id?: null | string;
  modifiedBy?: string;
  uploadedDate?: string;
  applicantType?: string;
}

export class FileUploadsForm {
  projectId = new UntypedFormControl();
  applicantType = new UntypedFormControl();
  deleteFlag = new UntypedFormControl();
  id = new UntypedFormControl();
  fileName = new UntypedFormControl();
  fileDescription = new UntypedFormControl();
  fileType = new UntypedFormControl();
  fileTypeText = new UntypedFormControl();
  requiredDocumentType = new UntypedFormControl();
  uploadedDate = new UntypedFormControl();
  modifiedBy = new UntypedFormControl();
  fileData = new UntypedFormControl();
  contentType = new UntypedFormControl();
  fileSize = new UntypedFormControl();
  prevEventConditionFileUpload: UntypedFormGroup;
  postEventConditionFileUpload: UntypedFormGroup;
  supportingFilesFileUpload: UntypedFormGroup;
  fileUploads = new UntypedFormControl([]);
  addNewFileUploadIndicator = new UntypedFormControl(false);

  constructor(
    fileUploads: Array<FileUpload>,
    customValidator: CustomValidationService,
    builder: UntypedFormBuilder
  ) {
    this.supportingFilesFileUpload = builder.group({
      deleteFlag: [
        false,
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      projectId: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      id: [
        '',
      ],
      fileName: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      fileDescription: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator),
          customValidator
            .maxLengthValidator(100)
            .bind(customValidator)
        ]
      ],
      fileType: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      fileTypeText: [
        ''
      ],
      requiredDocumentType: [
        ''
      ],
      uploadedDate: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      modifiedBy: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      fileData: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      contentType: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      fileSize: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ]
    });
    this.prevEventConditionFileUpload = builder.group({
      deleteFlag: [
        false,
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      projectId: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      id: [
        '',
      ],

      fileName: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      fileDescription: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator),
          customValidator
            .maxLengthValidator(100)
            .bind(customValidator)
        ]
      ],
      fileType: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      fileTypeText: [
        ''
      ],
      requiredDocumentType: [
        ''
      ],
      uploadedDate: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      modifiedBy: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      fileData: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      contentType: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      fileSize: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ]
    });
    this.postEventConditionFileUpload = builder.group({
      deleteFlag: [
        false,
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      projectId: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      id: [
        '',
      ],

      fileName: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      fileDescription: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator),
          customValidator
            .maxLengthValidator(100)
            .bind(customValidator)
        ]
      ],
      fileType: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      fileTypeText: [
        ''
      ],
      requiredDocumentType: [
        ''
      ],
      uploadedDate: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      modifiedBy: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      fileData: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      contentType: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ],
      fileSize: [
        '',
        [
          customValidator
            .conditionalValidation(
              () => this.addNewFileUploadIndicator.value,
              Validators.required
            )
            .bind(customValidator)
        ]
      ]
    });

  }
}

export class SupportingDocuments {
  hasCopyOfARentalAgreementOrLease?: boolean;

  constructor(
  ) { }
}

export class SupportingDocumentsForm {
  hasCopyOfARentalAgreementOrLease = new UntypedFormControl();

  constructor(
    supportingDocuments: SupportingDocuments,
  ) {
    if (supportingDocuments.hasCopyOfARentalAgreementOrLease != null) {
      this.hasCopyOfARentalAgreementOrLease.setValue(supportingDocuments.hasCopyOfARentalAgreementOrLease);
    }
    this.hasCopyOfARentalAgreementOrLease.setValidators(null);
  }
}

/**
 * DFA Application Main
 **/
export interface DfaProjectMain {
  id?: string;
  applicationId?: string;
  project?: RecoveryPlan;
}
