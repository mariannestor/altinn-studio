import {
  CircularProgress,
  createTheme,
  createStyles,
  makeStyles,
  Typography,
} from '@material-ui/core';
import classNames, { Argument } from 'classnames';
import React from 'react';
import altinnTheme from '../theme/altinnStudioTheme';

export interface IAltinnSpinnerComponentProvidedProps {
  id?: string;
  spinnerText?: any;
  styleObj?: Argument;
}

const theme = createTheme(altinnTheme);

const useStyles = makeStyles(() =>
  createStyles({
    spinner: {
      color: theme.altinnPalette.primary.blueDark,
      marginRight: 'auto',
      marginLeft: 'auto',
      display: 'inline-block',
    },
    spinnerText: {
      display: 'inline-block',
      fontSize: 16,
      marginLeft: '10px',
      verticalAlign: 'middle',
      marginBottom: '25px',
    },
  }),
);

const AltinnSpinner = (props: IAltinnSpinnerComponentProvidedProps) => {
  const classes = useStyles(props);

  return (
    <div className={classNames(props.styleObj)}>
      <CircularProgress
        className={classNames(classes.spinner)}
        id={props.id ? props.id : undefined}
      />
      {props.spinnerText && (
        <Typography className={classNames(classes.spinnerText)}>
          {props.spinnerText}
        </Typography>
      )}
    </div>
  );
};

export default AltinnSpinner;
