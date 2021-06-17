import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators
} from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { EvacueeProfileService } from 'src/app/core/services/evacuee-profile.service';
import { AlertService } from 'src/app/shared/components/alert/alert.service';
import { StepCreateProfileService } from '../../step-create-profile/step-create-profile.service';
import { WizardService } from '../../wizard.service';

import * as globalConst from 'src/app/core/services/global-constants';
import { CacheService } from 'src/app/core/services/cache.service';
import { first, map, mergeMap } from 'rxjs/operators';
import { RegistrationResult } from 'src/app/core/api/models';

@Component({
  selector: 'app-profile-review',
  templateUrl: './profile-review.component.html',
  styleUrls: ['./profile-review.component.scss']
})
export class ProfileReviewComponent implements OnInit, OnDestroy {
  verifiedProfileGroup: FormGroup = null;
  tabUpdateSubscription: Subscription;

  saveLoader = false;
  disableButton = false;

  constructor(
    private router: Router,
    private wizardService: WizardService,
    private evacueeProfileService: EvacueeProfileService,
    private alertService: AlertService,
    private formBuilder: FormBuilder,
    public stepCreateProfileService: StepCreateProfileService,
    private cacheService: CacheService
  ) {}

  ngOnInit(): void {
    // Set up form validation for verification check
    this.verifiedProfileGroup = this.formBuilder.group({
      verifiedProfile: [
        this.stepCreateProfileService.verifiedProfile,
        Validators.required
      ]
    });

    // Set "update tab status" method, called for any tab navigation
    this.tabUpdateSubscription = this.stepCreateProfileService.nextTabUpdate.subscribe(
      () => {
        this.updateTabStatus();
      }
    );
  }

  get verifiedProfileControl(): { [key: string]: AbstractControl } {
    return this.verifiedProfileGroup.controls;
  }

  /**
   * Go back to the Security Questions tab
   */
  back(): void {
    this.router.navigate([
      '/ess-wizard/create-evacuee-profile/security-questions'
    ]);
  }

  /**
   * Create or update evacuee profile and continue to Step 2
   */
  save(): void {
    this.stepCreateProfileService.nextTabUpdate.next();

    if (this.verifiedProfileGroup.valid) {
      this.saveLoader = true;

      // If profile's already been created, update existing record
      if (!this.stepCreateProfileService.registrantId) {
        this.evacueeProfileService
          .createProfile(this.stepCreateProfileService.createProfileDTO())
          .pipe(
            mergeMap((regResult) => {
              // Set Profile ID to Profile Service, retrieve profile from API
              this.stepCreateProfileService.registrantId = regResult.id;
              this.cacheService.set('primaryRegistrantId', regResult.id);

              return this.evacueeProfileService.getProfileFromId(
                this.stepCreateProfileService.registrantId
              );
            }),
            map((profile) => {
              // Update wizard's Profile Service with data from API
              this.stepCreateProfileService.getProfileDTO(
                this.stepCreateProfileService.registrantId,
                profile
              );
            })
          )
          .subscribe(
            () => {
              // Once all profile work is done, tell user save is complete and go to Step 2
              this.disableButton = true;
              this.saveLoader = false;

              this.stepCreateProfileService
                .openModal(
                  globalConst.evacueeProfileCreatedMessage.text,
                  globalConst.evacueeProfileCreatedMessage.title,
                  globalConst.evacueeProfileCreatedMessage.button
                )
                .afterClosed()
                .subscribe(() => {
                  this.wizardService.setStepStatus(
                    '/ess-wizard/create-ess-file',
                    false
                  );

                  this.router.navigate(['/ess-wizard/create-ess-file'], {
                    state: { step: 'STEP 2', title: 'Create ESS File' }
                  });
                });
            },
            (error) => {
              this.saveLoader = false;

              this.alertService.setAlert(
                'danger',
                globalConst.createRegProfileError
              );
            }
          );
      } else {
        //TODO: Update Profile code to go here
      }
    } else {
      this.verifiedProfileControl.verifiedProfile.markAsTouched();
    }
  }

  /**
   * Checks the form validity and updates the tab status
   */
  updateTabStatus() {
    if (this.verifiedProfileGroup.valid) {
      this.stepCreateProfileService.setTabStatus('review', 'complete');
    }

    this.stepCreateProfileService.verifiedProfile = this.verifiedProfileGroup.get(
      'verifiedProfile'
    ).value;
  }

  /**
   * When navigating away from tab, update variable value and status indicator
   */
  ngOnDestroy(): void {
    this.stepCreateProfileService.nextTabUpdate.next();
    this.tabUpdateSubscription.unsubscribe();
  }
}
