/* tslint:disable */
/* eslint-disable */
import { AppTypeInsurance } from './app-type-insurance';
import { Consent } from './consent';
import { OtherPreScreeningQuestions } from './other-pre-screening-questions';
import { ProfileVerification } from './profile-verification';

/**
 * Application
 */
export interface DfaApplicationStart {
  appTypeInsurance?: AppTypeInsurance;
  consent?: Consent;
  eventName?: null | string;
  id?: null | string;
  notifyUser?: boolean;
  otherPreScreeningQuestions?: OtherPreScreeningQuestions;
  profileVerification?: ProfileVerification;
}
