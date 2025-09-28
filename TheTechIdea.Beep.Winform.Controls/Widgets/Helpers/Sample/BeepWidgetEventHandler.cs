using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Sample
{
    /// <summary>
    /// Centralized event handling for all BeepWidget types
    /// Demonstrates BaseControl's hit area system integration
    /// </summary>
    public static class BeepWidgetEventHandler
    {
        /// <summary>
        /// Sets up comprehensive event handling for any BaseControl widget
        /// </summary>
        public static void SetupWidgetEvents(BaseControl widget)
        {
            // Metric widget events
            if (widget is BeepMetricWidget metricWidget)
            {
                metricWidget.ValueClicked += (sender, e) => 
                {
                    MessageBox.Show($"Value clicked: {metricWidget.Value} {metricWidget.Units}");
                };

                metricWidget.TrendClicked += (sender, e) => 
                {
                    MessageBox.Show($"Trend clicked: {metricWidget.TrendValue} ({metricWidget.TrendDirection})");
                };
            }

            // Chart widget events
            if (widget is BeepChartWidget chartWidget)
            {
                chartWidget.ChartClicked += (sender, e) => 
                {
                    MessageBox.Show($"Chart clicked: {chartWidget.Title}");
                };

                chartWidget.LegendClicked += (sender, e) => 
                {
                    MessageBox.Show("Legend clicked - could toggle data series");
                };
            }

            // List widget events
            if (widget is BeepListWidget listWidget)
            {
                listWidget.ItemClicked += (sender, e) => 
                {
                    MessageBox.Show($"List item clicked in: {listWidget.Title}");
                };

                listWidget.HeaderClicked += (sender, e) => 
                {
                    MessageBox.Show("Header clicked - could show sorting options");
                };
            }

            // Dashboard widget events
            if (widget is BeepDashboardWidget dashboardWidget)
            {
                dashboardWidget.PanelClicked += (sender, e) => 
                {
                    MessageBox.Show($"Dashboard panel clicked: {dashboardWidget.Title}");
                };
            }

            // Control widget events
            if (widget is BeepControlWidget controlWidget)
            {
                controlWidget.ValueChanged += (sender, e) => 
                {
                    MessageBox.Show($"Control value changed to: {controlWidget.Value}");
                };

                controlWidget.ControlClicked += (sender, e) => 
                {
                    MessageBox.Show($"Control clicked: {controlWidget.Title}");
                };
            }

            // Notification widget events
            if (widget is BeepNotificationWidget notificationWidget)
            {
                notificationWidget.ActionClicked += (sender, e) => 
                {
                    MessageBox.Show($"Notification action clicked: {notificationWidget.ActionText}");
                };

                notificationWidget.DismissClicked += (sender, e) => 
                {
                    MessageBox.Show("Notification dismissed");
                };
            }

            // Navigation widget events
            if (widget is BeepNavigationWidget navigationWidget)
            {
                navigationWidget.ItemClicked += (sender, e) => 
                {
                    MessageBox.Show($"Navigation item clicked: {((NavigationItem)e.EventData).Text}");
                };

                navigationWidget.NavigationChanged += (sender, e) => 
                {
                    MessageBox.Show($"Navigation changed to index: {e.EventData}");
                };
            }

            // Media widget events
            if (widget is BeepMediaWidget mediaWidget)
            {
                mediaWidget.MediaClicked += (sender, e) => 
                {
                    MessageBox.Show($"Media clicked: {mediaWidget.Title}");
                };

                mediaWidget.ImageClicked += (sender, e) => 
                {
                    MessageBox.Show($"Image clicked in: {mediaWidget.Title}");
                };

                mediaWidget.AvatarClicked += (sender, e) => 
                {
                    var item = e.EventData as MediaItem;
                    MessageBox.Show($"Avatar clicked: {item?.Title ?? "Unknown"}");
                };

                mediaWidget.OverlayClicked += (sender, e) => 
                {
                    MessageBox.Show($"Overlay clicked: {mediaWidget.OverlayText}");
                };
            }

            // Social widget events
            if (widget is BeepSocialWidget socialWidget)
            {
                socialWidget.UserClicked += (sender, e) => 
                {
                    MessageBox.Show($"User clicked: {socialWidget.UserName}");
                };

                socialWidget.AvatarClicked += (sender, e) => 
                {
                    MessageBox.Show($"Avatar clicked: {socialWidget.UserName}");
                };

                socialWidget.MessageClicked += (sender, e) => 
                {
                    MessageBox.Show($"Message clicked in: {socialWidget.Title}");
                };

                socialWidget.StatusClicked += (sender, e) => 
                {
                    MessageBox.Show($"Status clicked: {socialWidget.UserStatus}");
                };

                socialWidget.ActionClicked += (sender, e) => 
                {
                    var item = e.EventData as SocialItem;
                    MessageBox.Show($"Social item clicked: {item?.Name ?? "Unknown"}");
                };
            }

            // Finance widget events
            if (widget is BeepFinanceWidget financeWidget)
            {
                financeWidget.ValueClicked += (sender, e) => 
                {
                    MessageBox.Show($"Finance value clicked: {financeWidget.CurrencySymbol}{financeWidget.PrimaryValue:N2}");
                };

                financeWidget.TransactionClicked += (sender, e) => 
                {
                    MessageBox.Show($"Transaction clicked: {financeWidget.Title}");
                };

                financeWidget.AccountClicked += (sender, e) => 
                {
                    MessageBox.Show($"Account clicked: {financeWidget.AccountNumber}");
                };

                financeWidget.InvestmentClicked += (sender, e) => 
                {
                    var item = e.EventData as FinanceItem;
                    MessageBox.Show($"Investment clicked: {item?.Name ?? "Unknown"}");
                };

                financeWidget.ActionClicked += (sender, e) => 
                {
                    MessageBox.Show($"Finance action clicked: {financeWidget.Title}");
                };
            }

            // Map widget events
            if (widget is BeepMapWidget mapWidget)
            {
                mapWidget.LocationClicked += (sender, e) => 
                {
                    if (e.EventData is MapLocation location)
                    {
                        MessageBox.Show($"Location clicked: {location.Name} ({location.Address})");
                    }
                    else
                    {
                        MessageBox.Show($"Location clicked: {mapWidget.Address}");
                    }
                };

                mapWidget.RouteClicked += (sender, e) => 
                {
                    if (e.EventData is MapRoute route)
                    {
                        MessageBox.Show($"Route clicked: {route.Name} - {route.Distance:F1} km");
                    }
                    else
                    {
                        MessageBox.Show($"Route clicked in: {mapWidget.Title}");
                    }
                };

                mapWidget.MarkerClicked += (sender, e) => 
                {
                    MessageBox.Show($"Map marker clicked: {mapWidget.Title}");
                };

                mapWidget.AddressClicked += (sender, e) => 
                {
                    MessageBox.Show($"Address clicked: {mapWidget.Address}");
                };

                mapWidget.MapClicked += (sender, e) => 
                {
                    MessageBox.Show($"Map clicked: {mapWidget.Latitude:F4}, {mapWidget.Longitude:F4}");
                };
            }

            // Calendar widget events
            if (widget is BeepCalendarWidget calendarWidget)
            {
                calendarWidget.DateSelected += (sender, e) => 
                {
                    MessageBox.Show($"Date selected: {calendarWidget.SelectedDate:MMM dd, yyyy}");
                };

                calendarWidget.EventClicked += (sender, e) => 
                {
                    if (e.EventData is CalendarEvent calEvent)
                    {
                        MessageBox.Show($"Event clicked: {calEvent.Title} at {calEvent.StartTime:HH:mm}");
                    }
                    else
                    {
                        MessageBox.Show($"Event clicked in: {calendarWidget.Title}");
                    }
                };

                calendarWidget.TimeSlotClicked += (sender, e) => 
                {
                    if (e.EventData is TimeSlot timeSlot)
                    {
                        MessageBox.Show($"Time slot clicked: {timeSlot.Label} - {(timeSlot.IsAvailable ? "Available" : "Booked")}");
                    }
                    else
                    {
                        MessageBox.Show($"Time slot clicked in: {calendarWidget.Title}");
                    }
                };

                calendarWidget.MonthChanged += (sender, e) => 
                {
                    MessageBox.Show($"Month changed: {calendarWidget.DisplayMonth:MMMM yyyy}");
                };

                calendarWidget.ViewModeChanged += (sender, e) => 
                {
                    MessageBox.Show($"View mode changed: {calendarWidget.ViewMode}");
                };
            }

            // Form widget events
            if (widget is BeepFormWidget formWidget)
            {
                formWidget.FieldChanged += (sender, e) => 
                {
                    if (e.EventData is FormField field)
                    {
                        MessageBox.Show($"Field changed: {field.Label} = {field.Value}");
                    }
                    else
                    {
                        MessageBox.Show($"Field changed in: {formWidget.Title}");
                    }
                };

                formWidget.ValidationChanged += (sender, e) => 
                {
                    if (e.EventData is ValidationResult validation)
                    {
                        MessageBox.Show($"Validation: {validation.FieldName} - {(validation.IsValid ? "Valid" : validation.Message)}");
                    }
                    else if (e.EventData is bool isFormValid)
                    {
                        MessageBox.Show($"Form validation: {(isFormValid ? "All fields valid" : "Form has errors")}");
                    }
                    else
                    {
                        MessageBox.Show($"Validation changed in: {formWidget.Title}");
                    }
                };

                formWidget.StepChanged += (sender, e) => 
                {
                    MessageBox.Show($"Step changed: {formWidget.CurrentStep} of {formWidget.TotalSteps}");
                };

                formWidget.FormSubmitted += (sender, e) => 
                {
                    var formData = formWidget.GetFormData();
                    MessageBox.Show($"Form submitted: {formWidget.Title} with {formData.Count} fields");
                };

                formWidget.FormReset += (sender, e) => 
                {
                    MessageBox.Show($"Form reset: {formWidget.Title}");
                };
            }

            // BaseControl's HitDetected event for custom hit areas registered by painters
            widget.HitDetected += (sender, e) => 
            {
                MessageBox.Show($"Custom area clicked: {e.HitTest.Name}");
            };
        }
    }
}