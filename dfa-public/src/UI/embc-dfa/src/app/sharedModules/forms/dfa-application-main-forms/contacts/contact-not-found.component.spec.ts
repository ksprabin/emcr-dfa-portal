import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContactNotFoundComponent } from './contact-not-found.component';

describe('ContactNotFoundComponent', () => {
  let component: ContactNotFoundComponent;
  let fixture: ComponentFixture<ContactNotFoundComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContactNotFoundComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ContactNotFoundComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
