/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { ApplicantSubtypeSubCategories } from '../models/applicant-subtype-sub-categories';
import { ApplicantSubtypes } from '../models/applicant-subtypes';
import { CurrentApplication } from '../models/current-application';
import { DfaApplicationMain } from '../models/dfa-application-main';
import { DfaApplicationStart } from '../models/dfa-application-start';
import { PdfApplicationData } from '../models/pdf-application-data';

@Injectable({
  providedIn: 'root',
})
export class ApplicationService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation applicationAddApplication
   */
  static readonly ApplicationAddApplicationPath = '/api/applications/create';

  /**
   * Create an application.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `applicationAddApplication()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  applicationAddApplication$Response(params: {

    /**
     * The application information
     */
    body: DfaApplicationStart
  }): Observable<StrictHttpResponse<string>> {

    const rb = new RequestBuilder(this.rootUrl, ApplicationService.ApplicationAddApplicationPath, 'post');
    if (params) {
      rb.body(params.body, 'application/json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<string>;
      })
    );
  }

  /**
   * Create an application.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `applicationAddApplication$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  applicationAddApplication(params: {

    /**
     * The application information
     */
    body: DfaApplicationStart
  }): Observable<string> {

    return this.applicationAddApplication$Response(params).pipe(
      map((r: StrictHttpResponse<string>) => r.body as string)
    );
  }

  /**
   * Path part for operation applicationUpdateApplication
   */
  static readonly ApplicationUpdateApplicationPath = '/api/applications/update';

  /**
   * Update an application.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `applicationUpdateApplication()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  applicationUpdateApplication$Response(params: {

    /**
     * The application information
     */
    body: DfaApplicationMain
  }): Observable<StrictHttpResponse<string>> {

    const rb = new RequestBuilder(this.rootUrl, ApplicationService.ApplicationUpdateApplicationPath, 'put');
    if (params) {
      rb.body(params.body, 'application/json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<string>;
      })
    );
  }

  /**
   * Update an application.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `applicationUpdateApplication$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  applicationUpdateApplication(params: {

    /**
     * The application information
     */
    body: DfaApplicationMain
  }): Observable<string> {

    return this.applicationUpdateApplication$Response(params).pipe(
      map((r: StrictHttpResponse<string>) => r.body as string)
    );
  }

  /**
   * Path part for operation applicationGetPdfApplicationData
   */
  static readonly ApplicationGetPdfApplicationDataPath = '/api/applications';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `applicationGetPdfApplicationData()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  applicationGetPdfApplicationData$Response(params: {
    body: DfaApplicationMain
  }): Observable<StrictHttpResponse<PdfApplicationData>> {

    const rb = new RequestBuilder(this.rootUrl, ApplicationService.ApplicationGetPdfApplicationDataPath, 'post');
    if (params) {
      rb.body(params.body, 'application/json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<PdfApplicationData>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `applicationGetPdfApplicationData$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  applicationGetPdfApplicationData(params: {
    body: DfaApplicationMain
  }): Observable<PdfApplicationData> {

    return this.applicationGetPdfApplicationData$Response(params).pipe(
      map((r: StrictHttpResponse<PdfApplicationData>) => r.body as PdfApplicationData)
    );
  }

  /**
   * Path part for operation applicationGetApplicationStart
   */
  static readonly ApplicationGetApplicationStartPath = '/api/applications/appstart/byId';

  /**
   * Get an application by Id.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `applicationGetApplicationStart()` instead.
   *
   * This method doesn't expect any request body.
   */
  applicationGetApplicationStart$Response(params?: {

    /**
     * The application Id.
     */
    applicationId?: string;
  }): Observable<StrictHttpResponse<DfaApplicationStart>> {

    const rb = new RequestBuilder(this.rootUrl, ApplicationService.ApplicationGetApplicationStartPath, 'get');
    if (params) {
      rb.query('applicationId', params.applicationId, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<DfaApplicationStart>;
      })
    );
  }

  /**
   * Get an application by Id.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `applicationGetApplicationStart$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  applicationGetApplicationStart(params?: {

    /**
     * The application Id.
     */
    applicationId?: string;
  }): Observable<DfaApplicationStart> {

    return this.applicationGetApplicationStart$Response(params).pipe(
      map((r: StrictHttpResponse<DfaApplicationStart>) => r.body as DfaApplicationStart)
    );
  }

  /**
   * Path part for operation applicationGetApplicationMain
   */
  static readonly ApplicationGetApplicationMainPath = '/api/applications/appmain/byId';

  /**
   * Get an application main by Id.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `applicationGetApplicationMain()` instead.
   *
   * This method doesn't expect any request body.
   */
  applicationGetApplicationMain$Response(params?: {

    /**
     * The application Id.
     */
    applicationId?: string;
  }): Observable<StrictHttpResponse<DfaApplicationMain>> {

    const rb = new RequestBuilder(this.rootUrl, ApplicationService.ApplicationGetApplicationMainPath, 'get');
    if (params) {
      rb.query('applicationId', params.applicationId, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<DfaApplicationMain>;
      })
    );
  }

  /**
   * Get an application main by Id.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `applicationGetApplicationMain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  applicationGetApplicationMain(params?: {

    /**
     * The application Id.
     */
    applicationId?: string;
  }): Observable<DfaApplicationMain> {

    return this.applicationGetApplicationMain$Response(params).pipe(
      map((r: StrictHttpResponse<DfaApplicationMain>) => r.body as DfaApplicationMain)
    );
  }

  /**
   * Path part for operation applicationGetDfaApplications
   */
  static readonly ApplicationGetDfaApplicationsPath = '/api/applications/dfaapplication';

  /**
   * get dfa applications.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `applicationGetDfaApplications()` instead.
   *
   * This method doesn't expect any request body.
   */
  applicationGetDfaApplications$Response(params?: {
  }): Observable<StrictHttpResponse<Array<CurrentApplication>>> {

    const rb = new RequestBuilder(this.rootUrl, ApplicationService.ApplicationGetDfaApplicationsPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<CurrentApplication>>;
      })
    );
  }

  /**
   * get dfa applications.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `applicationGetDfaApplications$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  applicationGetDfaApplications(params?: {
  }): Observable<Array<CurrentApplication>> {

    return this.applicationGetDfaApplications$Response(params).pipe(
      map((r: StrictHttpResponse<Array<CurrentApplication>>) => r.body as Array<CurrentApplication>)
    );
  }

  /**
   * Path part for operation applicationGetApplicationDetailsForProject
   */
  static readonly ApplicationGetApplicationDetailsForProjectPath = '/api/applications/dfaapplicationbyID';

  /**
   * get dfa applications.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `applicationGetApplicationDetailsForProject()` instead.
   *
   * This method doesn't expect any request body.
   */
  applicationGetApplicationDetailsForProject$Response(params?: {

    /**
     * The application Id.
     */
    applicationId?: string;
  }): Observable<StrictHttpResponse<CurrentApplication>> {

    const rb = new RequestBuilder(this.rootUrl, ApplicationService.ApplicationGetApplicationDetailsForProjectPath, 'get');
    if (params) {
      rb.query('applicationId', params.applicationId, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<CurrentApplication>;
      })
    );
  }

  /**
   * get dfa applications.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `applicationGetApplicationDetailsForProject$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  applicationGetApplicationDetailsForProject(params?: {

    /**
     * The application Id.
     */
    applicationId?: string;
  }): Observable<CurrentApplication> {

    return this.applicationGetApplicationDetailsForProject$Response(params).pipe(
      map((r: StrictHttpResponse<CurrentApplication>) => r.body as CurrentApplication)
    );
  }

  /**
   * Path part for operation applicationGetApplicantSubTypes
   */
  static readonly ApplicationGetApplicantSubTypesPath = '/api/applications/applicantsubtypes';

  /**
   * Get the applicant subtype records.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `applicationGetApplicantSubTypes()` instead.
   *
   * This method doesn't expect any request body.
   */
  applicationGetApplicantSubTypes$Response(params?: {
  }): Observable<StrictHttpResponse<Array<ApplicantSubtypes>>> {

    const rb = new RequestBuilder(this.rootUrl, ApplicationService.ApplicationGetApplicantSubTypesPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<ApplicantSubtypes>>;
      })
    );
  }

  /**
   * Get the applicant subtype records.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `applicationGetApplicantSubTypes$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  applicationGetApplicantSubTypes(params?: {
  }): Observable<Array<ApplicantSubtypes>> {

    return this.applicationGetApplicantSubTypes$Response(params).pipe(
      map((r: StrictHttpResponse<Array<ApplicantSubtypes>>) => r.body as Array<ApplicantSubtypes>)
    );
  }

  /**
   * Path part for operation applicationGetApplicantSubSubTypes
   */
  static readonly ApplicationGetApplicantSubSubTypesPath = '/api/applications/applicantsubsubtypes';

  /**
   * Get the applicant subtype records.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `applicationGetApplicantSubSubTypes()` instead.
   *
   * This method doesn't expect any request body.
   */
  applicationGetApplicantSubSubTypes$Response(params?: {
  }): Observable<StrictHttpResponse<ApplicantSubtypeSubCategories>> {

    const rb = new RequestBuilder(this.rootUrl, ApplicationService.ApplicationGetApplicantSubSubTypesPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ApplicantSubtypeSubCategories>;
      })
    );
  }

  /**
   * Get the applicant subtype records.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `applicationGetApplicantSubSubTypes$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  applicationGetApplicantSubSubTypes(params?: {
  }): Observable<ApplicantSubtypeSubCategories> {

    return this.applicationGetApplicantSubSubTypes$Response(params).pipe(
      map((r: StrictHttpResponse<ApplicantSubtypeSubCategories>) => r.body as ApplicantSubtypeSubCategories)
    );
  }

}
