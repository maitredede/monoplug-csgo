using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSServerPlugin
{
    internal enum ERROR : uint
    {
        //general
        ok = 0x0000,
        undefined = 0x0001,
        not_implemented = 0x0002,
        ok_no_update = 0x0003,
        dont_notify = 0x0004,

        //dunno
        command_not_found = 0x0100,
        unable_to_bind_network_port = 0x0101,
        no_network_port_available = 0x0102,

        //client
        client_invalid_id = 0x0200,
        client_nickname_inuse = 0x0201,
        client_protocol_limit_reached = 0x0203,
        client_invalid_type = 0x0204,
        client_already_subscribed = 0x0205,
        client_not_logged_in = 0x0206,
        client_could_not_validate_identity = 0x0207,
        client_version_outdated = 0x020a,

        //channel
        channel_invalid_id = 0x0300,
        channel_protocol_limit_reached = 0x0301,
        channel_already_in = 0x0302,
        channel_name_inuse = 0x0303,
        channel_not_empty = 0x0304,
        channel_can_not_delete_default = 0x0305,
        channel_default_require_permanent = 0x0306,
        channel_invalid_flags = 0x0307,
        channel_parent_not_permanent = 0x0308,
        channel_maxclients_reached = 0x0309,
        channel_maxfamily_reached = 0x030a,
        channel_invalid_order = 0x030b,
        channel_no_filetransfer_supported = 0x030c,
        channel_invalid_password = 0x030d,

        //server
        server_invalid_id = 0x0400,
        server_running = 0x0401,
        server_is_shutting_down = 0x0402,
        server_maxclients_reached = 0x0403,
        server_invalid_password = 0x0404,
        server_is_virtual = 0x0407,
        server_is_not_running = 0x0409,

        //parameter
        parameter_quote = 0x0600,
        parameter_invalid_count = 0x0601,
        parameter_invalid = 0x0602,
        parameter_not_found = 0x0603,
        parameter_convert = 0x0604,
        parameter_invalid_size = 0x0605,
        parameter_missing = 0x0606,

        //unsorted, need further investigation
        vs_critical = 0x0700,
        connection_lost = 0x0701,
        not_connected = 0x0702,
        no_cached_connection_info = 0x0703,
        currently_not_possible = 0x0704,
        failed_connection_initialisation = 0x0705,
        could_not_resolve_hostname = 0x0706,
        invalid_server_connection_handler_id = 0x0707,
        could_not_initialise_input_manager = 0x0708,
        clientlibrary_not_initialised = 0x0709,
        serverlibrary_not_initialised = 0x070a,

        //sound
        sound_preprocessor_disabled = 0x0900,
        sound_internal_preprocessor = 0x0901,
        sound_internal_encoder = 0x0902,
        sound_internal_playback = 0x0903,
        sound_no_capture_device_available = 0x0904,
        sound_no_playback_device_available = 0x0905,
        sound_could_not_open_capture_device = 0x0906,
        sound_could_not_open_playback_device = 0x0907,
        sound_handler_has_device = 0x0908,
        sound_invalid_capture_device = 0x0909,
        sound_invalid_playback_device = 0x090a,

        //accounting
        accounting_virtualserver_limit_reached = 0x0b00,
        accounting_slot_limit_reached = 0x0b01,
        accounting_license_file_not_found = 0x0b02,
        accounting_license_date_not_ok = 0x0b03,
        accounting_unable_to_connect_to_server = 0x0b04,
        accounting_unknown_error = 0x0b05,
        accounting_server_error = 0x0b06,
        accounting_instance_limit_reached = 0x0b07,
        accounting_instance_check_error = 0x0b08,
        accounting_license_file_invalid = 0x0b09,
        accounting_running_elsewhere = 0x0b0a,
        accounting_instance_duplicated = 0x0b0b,
        accounting_already_started = 0x0b0c,
        accounting_not_started = 0x0b0d,
    }
}