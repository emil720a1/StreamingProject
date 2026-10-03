import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { Shell } from './shell';
import { AuthService } from '../../core/services/auth';

describe('Shell', () => {
  let component: Shell;
  let fixture: ComponentFixture<Shell>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Shell],
      providers: [
        provideRouter([]),
        {
          provide: AuthService,
          useValue: {
            logout: vi.fn(),
          }
        }],
    }).compileComponents();

    fixture = TestBed.createComponent(Shell);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
