using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class DesignerHelper
    { // Dictionary mapping a unique identifier (GUID) to the Control instance.
        private static readonly Dictionary<string, Control> _addedControls = new Dictionary<string, Control>();

        /// <summary>
        /// Adds a new control of type T to the specified container at design time.
        /// </summary>
        /// <typeparam name="T">Type of the control to add (must derive from Control and have a parameterless constructor).</typeparam>
        /// <param name="container">A container control (like a Form or Panel).</param>
        /// <returns>The added control instance.</returns>
        public static T AddControl<T>(Control container) where T : Control, new()
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            // Try to obtain the design–time host from the container's site.
            IDesignerHost host = container.Site?.GetService(typeof(IDesignerHost)) as IDesignerHost;
            T control = null;
            if (host != null)
            {
                // Start a designer transaction.
                DesignerTransaction transaction = host.CreateTransaction("Add Control");
                try
                {
                    // Create the control using the designer host.
                    control = host.CreateComponent(typeof(T)) as T;
                    if (control == null)
                        throw new InvalidOperationException("Failed to create the control using IDesignerHost.");

                    // Set any default properties (adjust as necessary).
                    control.Location = new System.Drawing.Point(10, 10);
                    control.Size = new System.Drawing.Size(100, 30);

                    // Add the control to the container.
                    container.Controls.Add(control);

                    // Commit the transaction so that the changes are serialized to the designer.cs.
                    transaction.Commit();
                }
                catch
                {
                    // Cancel transaction if an error occurs.
                    transaction.Cancel();
                    throw;
                }
            }
            else
            {
                // Fallback: if no designer host is available (e.g. at runtime),
                // just create and add the control.
                control = new T
                {
                    Location = new System.Drawing.Point(10, 10),
                    Size = new System.Drawing.Size(100, 30)
                };
                container.Controls.Add(control);
            }
            return control;
        }

        /// <summary>
        /// Removes a control from the container at design time.
        /// </summary>
        /// <param name="container">The container control from which to remove the control.</param>
        /// <param name="control">The control to be removed.</param>
        public static void RemoveControl(Control container, Control control)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            // Try to obtain the design–time host.
            IDesignerHost host = container.Site?.GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (host != null)
            {
                // Begin a designer transaction.
                DesignerTransaction transaction = host.CreateTransaction("Remove Control");
                try
                {
                    // Remove the control from the container.
                    container.Controls.Remove(control);

                    // Notify the host so that the component is properly removed (and the designer.cs is updated).
                    host.DestroyComponent(control);

                    // Commit the changes.
                    transaction.Commit();
                }
                catch
                {
                    transaction.Cancel();
                    throw;
                }
            }
            else
            {
                // Fallback: if no designer host exists, perform a normal removal.
                container.Controls.Remove(control);
                control.Dispose();
            }
        }

        /// <summary>
        /// Adds a new control to the specified container at design time using a type parameter.
        /// </summary>
        /// <param name="container">The container control (such as a Panel or Form) that is already on the design surface.</param>
        /// <param name="controlType">The System.Type of the control to add. It must derive from System.Windows.Forms.Control.</param>
        /// <returns>The newly added Control.</returns>
        public static Control AddControl(Control container, Type controlType)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (controlType == null)
                throw new ArgumentNullException(nameof(controlType));

            // Verify that the controlType is a Control.
            if (!typeof(Control).IsAssignableFrom(controlType))
                throw new ArgumentException("The provided type must derive from System.Windows.Forms.Control", nameof(controlType));

            // Try to obtain the design–time host from the container's Site.
            IDesignerHost host = container.Site?.GetService(typeof(IDesignerHost)) as IDesignerHost;
            Control newControl = null;

            if (host != null)
            {
                // Create a designer transaction to make the operation undoable.
                DesignerTransaction transaction = host.CreateTransaction("Add Control");
                try
                {
                    // Create an instance of the control via IDesignerHost. This ensures that the control is tracked by the design environment.
                    IComponent component = host.CreateComponent(controlType);
                    newControl = component as Control;
                    if (newControl == null)
                        throw new InvalidOperationException($"Unable to create an instance of {controlType.FullName}.");

                    // Optionally set default properties. Customize as needed.
                    newControl.Location = new System.Drawing.Point(10, 10);
                    newControl.Size = new System.Drawing.Size(100, 30);

                    // Add the new control to the container.
                    container.Controls.Add(newControl);

                    // Commit the changes so that the designer generates the corresponding code.
                    transaction.Commit();
                }
                catch
                {
                    transaction.Cancel();
                    throw;
                }
            }
            else
            {
                // Fallback for runtime or when not in design mode
                newControl = (Control)Activator.CreateInstance(controlType);
                newControl.Location = new System.Drawing.Point(10, 10);
                newControl.Size = new System.Drawing.Size(100, 30);
                container.Controls.Add(newControl);
            }

            return newControl;
        }
        /// <summary>
        /// Adds a new control of the specified type to the container at design time.
        /// A unique GUID is generated and stored in the control's Tag property,
        /// and the control is tracked in an internal dictionary for later removal.
        /// </summary>
        /// <param name="container">The container control (for example, a form or panel) on the design surface.</param>
        /// <param name="controlType">The type of control to add. It must derive from System.Windows.Forms.Control.</param>
        /// <param name="controlGuid">Returns the generated GUID for the new control.</param>
        /// <returns>The created Control instance.</returns>
        public static Control AddControl(Control container, Type controlType, out string controlGuid)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (controlType == null)
                throw new ArgumentNullException(nameof(controlType));

            if (!typeof(Control).IsAssignableFrom(controlType))
                throw new ArgumentException("controlType must derive from System.Windows.Forms.Control", nameof(controlType));

            Control newControl = null;
            // Generate a new unique identifier.
            controlGuid = Guid.NewGuid().ToString();

            // Try to obtain the design–time host.
            IDesignerHost host = container.Site?.GetService(typeof(IDesignerHost)) as IDesignerHost;

            if (host != null)
            {
                DesignerTransaction transaction = host.CreateTransaction("Add Control");
                try
                {
                    // Create the control via the designer host so that it is tracked and serialized.
                    IComponent component = host.CreateComponent(controlType);
                    newControl = component as Control;
                    if (newControl == null)
                        throw new InvalidOperationException("Could not create control instance using IDesignerHost.");

                    // Set default properties. Adjust these as needed.
                    newControl.Location = new Point(10, 10);
                    newControl.Size = new Size(100, 30);

                    // Store the GUID in the control's Tag so you can later use it to identify the control.
                    newControl.Tag = controlGuid;

                    // Add the new control to the container and record it in the dictionary.
                    container.Controls.Add(newControl);
                    _addedControls.Add(controlGuid, newControl);

                    // Commit the designer transaction.
                    transaction.Commit();
                }
                catch
                {
                    transaction.Cancel();
                    throw;
                }
            }
            else
            {
                // Fallback for runtime or if design services are not available.
                newControl = (Control)Activator.CreateInstance(controlType);
                newControl.Location = new Point(10, 10);
                newControl.Size = new Size(100, 30);
                newControl.Tag = controlGuid;
                container.Controls.Add(newControl);
                _addedControls.Add(controlGuid, newControl);
            }

            return newControl;
        }

        /// <summary>
        /// Removes a control from the container based on its unique identifier (GUID).
        /// The method looks up the control in the tracking dictionary and, if found,
        /// removes and destroys it using the design–time host when available.
        /// </summary>
        /// <param name="container">The container control from which to remove the control.</param>
        /// <param name="controlGuid">The GUID of the control to remove.</param>
        public static void RemoveControl(Control container, string controlGuid)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            if (string.IsNullOrEmpty(controlGuid))
                throw new ArgumentException("controlGuid must be a valid non-empty string.", nameof(controlGuid));

            // Check whether the control is tracked.
            if (!_addedControls.ContainsKey(controlGuid))
                return;

            Control controlToRemove = _addedControls[controlGuid];
            // Try to obtain the design-time host.
            IDesignerHost host = container.Site?.GetService(typeof(IDesignerHost)) as IDesignerHost;

            if (host != null)
            {
                DesignerTransaction transaction = host.CreateTransaction("Remove Control");
                try
                {
                    container.Controls.Remove(controlToRemove);
                    // Inform the design environment that this component is being removed.
                    host.DestroyComponent(controlToRemove);
                    _addedControls.Remove(controlGuid);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Cancel();
                    throw;
                }
            }
            else
            {
                // Fallback: normal removal.
                container.Controls.Remove(controlToRemove);
                controlToRemove.Dispose();
                _addedControls.Remove(controlGuid);
            }
        }
    }
}
