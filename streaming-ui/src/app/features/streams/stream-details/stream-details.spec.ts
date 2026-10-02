import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { StreamDetails } from './stream-details';

describe('StreamDetails', () => {
  let component: StreamDetails;
  let fixture: ComponentFixture<StreamDetails>;
  let routeId = '1';

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StreamDetails],
      providers: [
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (key: string) => {
                  return key === 'id' ? routeId : null;
                },
              },
            },
          },
        },
      ],
    }).compileComponents();
  });

  it('should create', () => {
    fixture = TestBed.createComponent(StreamDetails);
    component = fixture.componentInstance;

    expect(component).toBeTruthy();
  });

  it('should load stream by route id', () => {
    routeId = '1';

    fixture = TestBed.createComponent(StreamDetails);
    component = fixture.componentInstance;

    expect(component.stream?.title).toBe('Gaming Live');
    expect(component.stream?.author).toBe('Alex');
  });

  it('should show not found state for unknown stream', () => {
    routeId = '999';

    fixture = TestBed.createComponent(StreamDetails);
    fixture.detectChanges();

    const html = fixture.nativeElement as HTMLElement;

    expect(html.textContent).toContain('Stream not found');
  });
});
