import React from 'react';
import configureStore from 'redux-mock-store';
import userEvent from '@testing-library/user-event';
import { render } from '@testing-library/react';
import { Provider } from 'react-redux';

export const renderWithRedux = (element: JSX.Element) => {
  const store = configureStore()({});
  const user = userEvent.setup();
  render(<Provider store={store}>{element}</Provider>);
  return { store, user };
};
