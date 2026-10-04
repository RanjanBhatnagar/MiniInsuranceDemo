import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './app.html',
  // styleUrl: './app.css'
})
export class App {

  username = 'admin';
  password = 'admin123';

  token = signal('');
  loggedIn = signal(false);

  customers = signal<any[]>([]);
  policies = signal<any[]>([]);

  message = signal('');

  async login() {

    console.log('Login Function Entered');

    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        username: this.username,
        password: this.password
      })
    });

    const data = await response.json();

    console.log('Login Response:', data);

    if (!response.ok) {
      this.message.set(data.message ?? 'Login failed');
      return;
    }

    this.token.set(data.token);
    this.loggedIn.set(true);
    this.message.set('Login successful');

    console.log('JWT stored');
  }

  async loadCustomers() {

    console.log('Loading customers...');

    const response = await fetch('/api/customers', {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${this.token()}`
      }
    });

    console.log('Customer status:', response.status);

    if (!response.ok) {
      const errorText = await response.text();
      console.error('Customer error:', errorText);
      this.message.set(`Customer API error: ${response.status}`);
      return;
    }

    const data = await response.json();

    console.log('Customer data:', data);

    this.customers.set(data);

    console.log('Customers signal:', this.customers());
  }

  async loadPolicies() {

    console.log('Loading policies...');

    const response = await fetch('/api/policies', {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${this.token()}`
      }
    });

    console.log('Policy status:', response.status);

    if (!response.ok) {
      const errorText = await response.text();
      console.error('Policy error:', errorText);
      this.message.set(`Policy API error: ${response.status}`);
      return;
    }

    const data = await response.json();

    console.log('Policy data:', data);

    this.policies.set(data);

    console.log('Policies signal:', this.policies());
  }
}