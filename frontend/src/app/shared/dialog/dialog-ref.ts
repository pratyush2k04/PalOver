import { Subject } from 'rxjs';

export class DialogRef<R = unknown> {
  private readonly closedSubject = new Subject<R | undefined>();
  readonly closed$ = this.closedSubject.asObservable();

  close(result?: R): void {
    this.closedSubject.next(result);
    this.closedSubject.complete();
  }

  afterClosed() {
    return this.closedSubject.asObservable();
  }
}
