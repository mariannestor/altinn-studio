import React from 'react';
import * as networking from 'app-shared/utils/networking';

import { HandleMergeConflictDiscardChanges } from './HandleMergeConflictDiscardChanges';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { ClassNameMap } from '@material-ui/core/styles/withStyles';

const consoleError = jest
  .spyOn(console, 'error')
  .mockImplementation(() => 'error');

const renderHandleMergeConflictDiscardChanges = () => {
  const user = userEvent.setup();
  const classes: ClassNameMap = {};
  const container = render(
    <HandleMergeConflictDiscardChanges classes={classes} language={{}} />,
  );
  return { container, user };
};

test('should handle successfully returned data from API', async () => {
  const { user } = renderHandleMergeConflictDiscardChanges();

  // Mocks
  const mockGet = jest
    .spyOn(networking, 'get')
    .mockImplementationOnce(() => Promise.resolve());

  const discardMergeChangesBtn = screen.getByRole('button', {
    name: 'handle_merge_conflict.discard_changes_button',
  });

  // Expect discard button to exist
  expect(discardMergeChangesBtn).toBeDefined();

  // Expect this button to open the popup
  expect(screen.queryAllByRole('presentation')).toHaveLength(0);
  await user.click(discardMergeChangesBtn);
  expect(screen.getByRole('presentation')).toBeDefined();

  const discardMergeChangesConfirmBtn = screen.getByRole('button', {
    name: 'handle_merge_conflict.discard_changes_button_confirm',
  });
  expect(discardMergeChangesConfirmBtn).toBeDefined();
  await user.click(discardMergeChangesConfirmBtn);
  expect(mockGet).toHaveBeenCalled();
});

test('should handle unsuccessfully returned data from API', async () => {
  const { user } = renderHandleMergeConflictDiscardChanges();

  // Mocks
  const mockGet = jest
    .spyOn(networking, 'get')
    .mockImplementationOnce(() => Promise.reject());

  const discardMergeChangesBtn = screen.getByRole('button', {
    name: 'handle_merge_conflict.discard_changes_button',
  });

  await user.click(discardMergeChangesBtn);

  const discardMergeChangesConfirmBtn = screen.getByRole('button', {
    name: 'handle_merge_conflict.discard_changes_button_confirm',
  });

  await user.click(discardMergeChangesConfirmBtn);
  expect(mockGet).toHaveBeenCalled();
  expect(consoleError).toHaveBeenCalled();
});
