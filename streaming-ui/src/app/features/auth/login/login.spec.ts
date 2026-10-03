import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { Login } from './login';

describe('Login', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Login],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show error when email is empty', () => {
    component.email = '';
    component.password = 'password123';

    component.submit();

    expect(component.errorMessage).toBe('Email is required');
  });

  it('should show error when password is empty', () => {
    component.email = 'user@gmail.com'
    component.password = '';

    component.submit();

    expect(component.errorMessage).toBe('Password is required');
  })

  it('should accept valid credentials', () => {
    component.email = 'user@gmail.com';
    component.password = 'password123';

    component.submit();

    expect(component.errorMessage).toBe('');
  })
});
