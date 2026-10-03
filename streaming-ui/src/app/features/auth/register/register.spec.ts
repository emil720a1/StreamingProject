import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';

import { AuthService } from '../../../core/services/auth';
import { Register } from './register';

describe('Register', () => {
  let component: Register;
  let fixture: ComponentFixture<Register>;

  const authServiceMock = {
    register: () =>
      of({
        id: '1',
        username: 'alex',
        email: 'alex@example.com',
      }),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Register],
      providers: [
        provideRouter([
          {
            path: 'login',
            children: [],
          },
        ]),
        {
          provide: AuthService,
          useValue: authServiceMock,
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Register);
    component = fixture.componentInstance;

    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show error when username is empty', () => {
    component.username = '';
    component.email = 'alex@example.com';
    component.password = 'password123';
    component.confirmPassword = 'password123';

    component.submit();

    expect(component.errorMessage).toBe('Username is required');
  });

  it('should show error when email is empty', () => {
    component.username = 'alex';
    component.email = '';
    component.password = 'password123';
    component.confirmPassword = 'password123';

    component.submit();

    expect(component.errorMessage).toBe('Email is required');
  });

  it('should show error when password is empty', () => {
    component.username = 'alex';
    component.email = 'alex@example.com';
    component.password = '';
    component.confirmPassword = '';

    component.submit();

    expect(component.errorMessage).toBe('Password is required');
  });

  it('should show error when passwords do not match', () => {
    component.username = 'alex';
    component.email = 'alex@example.com';
    component.password = 'password123';
    component.confirmPassword = 'different-password';

    component.submit();

    expect(component.errorMessage).toBe('Passwords do not match');
  });

  it('should accept valid registration data', () => {
    component.username = 'alex';
    component.email = 'alex@example.com';
    component.password = 'password123';
    component.confirmPassword = 'password123';

    component.submit();

    expect(component.errorMessage).toBe('');
    expect(component.isSubmitting).toBe(false);
  });
});
