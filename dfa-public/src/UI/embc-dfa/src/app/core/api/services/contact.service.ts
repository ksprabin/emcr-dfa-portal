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

import { BCeIdBusiness } from '../models/b-ce-id-business';
import { BceidUserData } from '../models/bceid-user-data';

@Injectable({
  providedIn: 'root',
})
export class ContactService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation contactGetDashboardContactInfo
   */
  static readonly ContactGetDashboardContactInfoPath = '/api/contacts/dashboard';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `contactGetDashboardContactInfo()` instead.
   *
   * This method doesn't expect any request body.
   */
  contactGetDashboardContactInfo$Response(params?: {
  }): Observable<StrictHttpResponse<BCeIdBusiness>> {

    const rb = new RequestBuilder(this.rootUrl, ContactService.ContactGetDashboardContactInfoPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<BCeIdBusiness>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `contactGetDashboardContactInfo$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  contactGetDashboardContactInfo(params?: {
  }): Observable<BCeIdBusiness> {

    return this.contactGetDashboardContactInfo$Response(params).pipe(
      map((r: StrictHttpResponse<BCeIdBusiness>) => r.body as BCeIdBusiness)
    );
  }

  /**
   * Path part for operation contactGetLoginInfo
   */
  static readonly ContactGetLoginInfoPath = '/api/contacts/getlogin';

  /**
   * If user isn't authenticated, return NULL.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `contactGetLoginInfo()` instead.
   *
   * This method doesn't expect any request body.
   */
  contactGetLoginInfo$Response(params?: {
  }): Observable<StrictHttpResponse<BceidUserData>> {

    const rb = new RequestBuilder(this.rootUrl, ContactService.ContactGetLoginInfoPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<BceidUserData>;
      })
    );
  }

  /**
   * If user isn't authenticated, return NULL.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `contactGetLoginInfo$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  contactGetLoginInfo(params?: {
  }): Observable<BceidUserData> {

    return this.contactGetLoginInfo$Response(params).pipe(
      map((r: StrictHttpResponse<BceidUserData>) => r.body as BceidUserData)
    );
  }

  /**
   * Path part for operation contactCreateAuditEvent
   */
  static readonly ContactCreateAuditEventPath = '/api/contacts/createaudit';

  /**
   * Gets the same BCeID user info as getlogin, but also ensures that a dfa_bceidusers record exists
   * for the current logged in user.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `contactCreateAuditEvent()` instead.
   *
   * This method doesn't expect any request body.
   */
  contactCreateAuditEvent$Response(params?: {

    /**
     * Brief description of event type; eg. &quot;login&quot;, or &quot;submit Application&quot;
     */
    eventMoniker?: string;
  }): Observable<StrictHttpResponse<string>> {

    const rb = new RequestBuilder(this.rootUrl, ContactService.ContactCreateAuditEventPath, 'get');
    if (params) {
      rb.query('eventMoniker', params.eventMoniker, {});
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
   * Gets the same BCeID user info as getlogin, but also ensures that a dfa_bceidusers record exists
   * for the current logged in user.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `contactCreateAuditEvent$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  contactCreateAuditEvent(params?: {

    /**
     * Brief description of event type; eg. &quot;login&quot;, or &quot;submit Application&quot;
     */
    eventMoniker?: string;
  }): Observable<string> {

    return this.contactCreateAuditEvent$Response(params).pipe(
      map((r: StrictHttpResponse<string>) => r.body as string)
    );
  }

}
